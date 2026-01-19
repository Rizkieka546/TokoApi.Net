using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using TokoApi.Data;
using TokoApi.DTOs.Requests;
using TokoApi.DTOs.Responses;
using TokoApi.Models;
using TokoApi.Responses;

namespace TokoApi.Services;

public class ProductService
{
    private readonly AppDbContext _db;
    private readonly Cloudinary _cloudinary;

    public ProductService(AppDbContext db, CloudinaryConfig cloudinaryConfig)
    {
        _db = db;
        _cloudinary = cloudinaryConfig.Cloudinary;
    }

    public async Task<ApiResponse<PagedResult<ProductResponse>>> GetAll(
        int page,
        int pageSize,
        string? search,
        Guid? categoryId,
        bool isAdmin)
    {
        var query = _db.Products
            .Include(p => p.Category)
            .AsQueryable();

        if (!isAdmin)
            query = query.Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()));

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryName = p.Category.Name,
                ImageUrl = p.ImageUrl,
                Description = p.Description,
                IsActive = p.IsActive
            })
            .ToListAsync();

        var result = new PagedResult<ProductResponse>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };

        return ApiResponse<PagedResult<ProductResponse>>.Ok(result);
    }

    public async Task<ApiResponse<ProductResponse>> Create(ProductCreateRequest request)
    {
        bool categoryExist = await _db.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExist)
            return ApiResponse<ProductResponse>.Fail("Category tidak ditemukan");

        string imageUrl = await UploadImageToCloudinary(request.Image);

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price,
            CategoryId = request.CategoryId,
            Description = request.Description,
            ImageUrl = imageUrl,
            IsActive = true
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        return ApiResponse<ProductResponse>.Ok(new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            CategoryName = (await _db.Categories.FindAsync(product.CategoryId))!.Name,
            ImageUrl = product.ImageUrl,
            Description = product.Description,
            IsActive = product.IsActive
        }, "Product berhasil dibuat");
    }

    public async Task<ApiResponse<ProductResponse>> Update(Guid id, ProductUpdateRequest request)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
            return ApiResponse<ProductResponse>.Fail("Product tidak ditemukan");

        bool categoryExist = await _db.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExist)
            return ApiResponse<ProductResponse>.Fail("Category tidak ditemukan");

        product.Name = request.Name;
        product.Price = request.Price;
        product.CategoryId = request.CategoryId;
        product.Description = request.Description;

        if (request.Image != null)
        {
            string newImageUrl = await UploadImageToCloudinary(request.Image);
            product.ImageUrl = newImageUrl;
        }

        await _db.SaveChangesAsync();

        return ApiResponse<ProductResponse>.Ok(new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            CategoryName = (await _db.Categories.FindAsync(product.CategoryId))!.Name,
            ImageUrl = product.ImageUrl,
            Description = product.Description,
            IsActive = product.IsActive
        }, "Product berhasil diperbarui");
    }

    public async Task<ApiResponse<string>> Toggle(Guid id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
            return ApiResponse<string>.Fail("Product tidak ditemukan");

        product.IsActive = !product.IsActive;
        await _db.SaveChangesAsync();

        return ApiResponse<string>.Ok(null, "Status product berhasil diubah");
    }

    public async Task<ApiResponse<string>> Delete(Guid id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
            return ApiResponse<string>.Fail("Product tidak ditemukan");

        product.IsDeleted = true;
        await _db.SaveChangesAsync();

        return ApiResponse<string>.Ok(null, "Product berhasil dihapus");
    }

    private async Task<string> UploadImageToCloudinary(IFormFile file)
    {
        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "toko/products",
            UseFilename = false,
            UniqueFilename = true,
            Overwrite = false
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.StatusCode != System.Net.HttpStatusCode.OK)
            throw new Exception("Gagal upload gambar ke Cloudinary");

        return result.SecureUrl.ToString();
    }
}
