using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TokoApi.Data;
using TokoApi.DTOs.Requests;
using TokoApi.DTOs.Responses;
using TokoApi.Models;
using TokoApi.Responses;

namespace TokoApi.Services;

public class UserService
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher<User> _hasher = new();

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResponse<List<UserResponse>>> GetAll()
    {
        var users = await _db.Users
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();

        return ApiResponse<List<UserResponse>>.Ok(users);
    }

    public async Task<ApiResponse<UserResponse>> Create(UserCreateRequest request)
    {
        if (request.Role != "admin" && request.Role != "kasir")
            return ApiResponse<UserResponse>.Fail("Role tidak valid");

        bool emailExist = await _db.Users.AnyAsync(x => x.Email == request.Email);
        if (emailExist)
            return ApiResponse<UserResponse>.Fail("Email sudah digunakan");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            Role = request.Role
        };

        user.PasswordHash = _hasher.HashPassword(user, request.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return ApiResponse<UserResponse>.Ok(new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        }, "User berhasil dibuat");
    }

    public async Task<ApiResponse<UserResponse>> Update(Guid id, UserUpdateRequest request)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return ApiResponse<UserResponse>.Fail("User tidak ditemukan");

        if (request.Role != "admin" && request.Role != "kasir")
            return ApiResponse<UserResponse>.Fail("Role tidak valid");

        user.Username = request.Username;
        user.Role = request.Role;

        await _db.SaveChangesAsync();

        return ApiResponse<UserResponse>.Ok(new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        }, "User berhasil diperbarui");
    }

    public async Task<ApiResponse<string>> ResetPassword(Guid id, UserResetPasswordRequest request)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return ApiResponse<string>.Fail("User tidak ditemukan");

        user.PasswordHash = _hasher.HashPassword(user, request.NewPassword);
        await _db.SaveChangesAsync();

        return ApiResponse<string>.Ok(null, "Password berhasil direset");
    }

    public async Task<ApiResponse<string>> Delete(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return ApiResponse<string>.Fail("User tidak ditemukan");

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();

        return ApiResponse<string>.Ok(null, "User berhasil dihapus");
    }
}
