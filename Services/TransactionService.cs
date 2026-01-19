using Microsoft.EntityFrameworkCore;
using TokoApi.Data;
using TokoApi.DTOs.Requests;
using TokoApi.DTOs.Responses;
using TokoApi.Models;
using TokoApi.Responses;

namespace TokoApi.Services;

public class TransactionService
{
    private readonly AppDbContext _db;

    public TransactionService(AppDbContext db)
    {
        _db = db;
    }

    // CREATE transaksi (kasir)
    public async Task<ApiResponse<string>> Create(Guid kasirId, TransactionCreateRequest request)
    {
        if (request.Items.Count == 0)
            return ApiResponse<string>.Fail("Item transaksi kosong");

        var productIds = request.Items.Select(i => i.ProductId).ToList();

        var products = await _db.Products
            .Where(p => productIds.Contains(p.Id) && p.IsActive)
            .ToDictionaryAsync(p => p.Id, p => p);

        if (products.Count != productIds.Count)
            return ApiResponse<string>.Fail("Ada product tidak valid atau nonaktif");

        decimal total = 0;

        foreach (var item in request.Items)
        {
            total += products[item.ProductId].Price * item.Qty;
        }

        if (request.PaidAmount < total)
            return ApiResponse<string>.Fail("Uang bayar kurang");

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            KasirId = kasirId,
            TotalPrice = total,
            PaymentMethod = request.PaymentMethod,
            PaidAmount = request.PaidAmount,
            ChangeAmount = request.PaidAmount - total
        };

        _db.Transactions.Add(transaction);

        foreach (var item in request.Items)
        {
            _db.TransactionItems.Add(new TransactionItem
            {
                Id = Guid.NewGuid(),
                TransactionId = transaction.Id,
                ProductId = item.ProductId,
                Qty = item.Qty,
                PriceSnapshot = products[item.ProductId].Price
            });
        }

        await _db.SaveChangesAsync();

        return ApiResponse<string>.Ok(null, "Transaksi berhasil disimpan");
    }

    // GET list transaksi (admin)
    public async Task<ApiResponse<PagedResult<TransactionResponse>>> GetAll(
        int page, int pageSize)
    {
        var query = _db.Transactions.Include(t => t.Kasir);

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var items = await query
            .OrderByDescending(t => t.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TransactionResponse
            {
                Id = t.Id,
                KasirName = t.Kasir.Username,
                TotalPrice = t.TotalPrice,
                PaymentMethod = t.PaymentMethod,
                Date = t.Date
            })
            .ToListAsync();

        return ApiResponse<PagedResult<TransactionResponse>>.Ok(new PagedResult<TransactionResponse>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        });
    }

    // GET detail transaksi
    public async Task<ApiResponse<TransactionDetailResponse>> GetById(Guid id)
    {
        var transaction = await _db.Transactions
            .Include(t => t.Kasir)
            .Include(t => t.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction == null)
            return ApiResponse<TransactionDetailResponse>.Fail("Transaksi tidak ditemukan");

        var detail = new TransactionDetailResponse
        {
            Id = transaction.Id,
            KasirName = transaction.Kasir.Username,
            TotalPrice = transaction.TotalPrice,
            PaidAmount = transaction.PaidAmount,
            ChangeAmount = transaction.ChangeAmount,
            PaymentMethod = transaction.PaymentMethod,
            Date = transaction.Date,
            Items = transaction.Items.Select(i => new TransactionItemDetail
            {
                ProductName = i.Product.Name,
                Qty = i.Qty,
                PriceSnapshot = i.PriceSnapshot
            }).ToList()
        };

        return ApiResponse<TransactionDetailResponse>.Ok(detail);
    }
}
