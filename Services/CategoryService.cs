using Microsoft.EntityFrameworkCore;
using TokoApi.Data;
using TokoApi.DTOs.Requests;
using TokoApi.DTOs.Responses;
using TokoApi.Models;
using TokoApi.Responses;

namespace TokoApi.Services;

public class CategoryService
{
    private readonly AppDbContext _db;

    public CategoryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResponse<PagedResult<CategoryResponse>>> GetAll(
        int page, int pageSize, string? search)
    {
        var query = _db.Categories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.Name.ToLower().Contains(search.ToLower()));

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        var result = new PagedResult<CategoryResponse>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };

        return ApiResponse<PagedResult<CategoryResponse>>.Ok(result);
    }

    public async Task<ApiResponse<CategoryResponse>> Create(CategoryRequest request)
    {
        bool exist = await _db.Categories
            .AnyAsync(c => c.Name.ToLower() == request.Name.ToLower());

        if (exist)
            return ApiResponse<CategoryResponse>.Fail("Category sudah ada");

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return ApiResponse<CategoryResponse>.Ok(new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            CreatedAt = category.CreatedAt
        }, "Category berhasil dibuat");
    }

    public async Task<ApiResponse<CategoryResponse>> Update(Guid id, CategoryRequest request)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
            return ApiResponse<CategoryResponse>.Fail("Category tidak ditemukan");

        category.Name = request.Name;
        await _db.SaveChangesAsync();

        return ApiResponse<CategoryResponse>.Ok(new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            CreatedAt = category.CreatedAt
        }, "Category berhasil diperbarui");
    }

    // Soft Delete
    public async Task<ApiResponse<string>> Delete(Guid id)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
            return ApiResponse<string>.Fail("Category tidak ditemukan");

        category.IsDeleted = true;
        await _db.SaveChangesAsync();

        return ApiResponse<string>.Ok(null, "Category berhasil dihapus");
    }
}
