using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TokoApi.Data;
using TokoApi.DTOs.Requests;
using TokoApi.DTOs.Responses;
using TokoApi.Models;
using TokoApi.Responses;

namespace TokoApi.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly PasswordHasher<User> _hasher = new();

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<ApiResponse<UserInfoResponse>> Login(LoginRequest request, HttpResponse response)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
        if (user == null)
            return ApiResponse<UserInfoResponse>.Fail("Email atau password salah");

        var verify = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verify == PasswordVerificationResult.Failed)
            return ApiResponse<UserInfoResponse>.Fail("Email atau password salah");

        var jwt = GenerateJwt(user);

        bool isProduction = _config["ASPNETCORE_ENVIRONMENT"] == "Production";

        response.Cookies.Append("access_token", jwt, new CookieOptions
        {
            HttpOnly = true,
            Secure = isProduction,                
            SameSite = SameSiteMode.None,         
            Expires = DateTime.UtcNow.AddMinutes(
                _config.GetValue<int>("Jwt:ExpireMinutes")
            )
        });

        var userInfo = new UserInfoResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role
        };

        return ApiResponse<UserInfoResponse>.Ok(userInfo, "Login berhasil");
    }

    public Task Logout(HttpResponse response)
    {
        bool isProduction = _config["ASPNETCORE_ENVIRONMENT"] == "Production";

        response.Cookies.Delete("access_token", new CookieOptions
        {
            Secure = isProduction,
            SameSite = SameSiteMode.None
        });

        return Task.CompletedTask;
    }

    private string GenerateJwt(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim("username", user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                _config.GetValue<int>("Jwt:ExpireMinutes")
            ),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<ApiResponse<UserInfoResponse>> GetMe(HttpRequest request)
    {
        if (!request.Cookies.TryGetValue("access_token", out var token))
            return ApiResponse<UserInfoResponse>.Fail("Unauthorized");

        var handler = new JwtSecurityTokenHandler();

        try
        {
            var jwt = handler.ReadJwtToken(token);

            var userId = jwt.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;

            var user = await _db.Users.FindAsync(Guid.Parse(userId));
            if (user == null)
                return ApiResponse<UserInfoResponse>.Fail("Unauthorized");

            var userInfo = new UserInfoResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };

            return ApiResponse<UserInfoResponse>.Ok(userInfo);
        }
        catch
        {
            return ApiResponse<UserInfoResponse>.Fail("Unauthorized");
        }
    }
}
