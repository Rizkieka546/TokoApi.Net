using Microsoft.EntityFrameworkCore;
using TokoApi.Data;
using TokoApi.DTOs.Responses;
using TokoApi.Responses;

namespace TokoApi.Services;

public class ReportService
{
    private readonly AppDbContext _db;

    public ReportService(AppDbContext db)
    {
        _db = db;
    }

    // DAILY REPORT
    public async Task<ApiResponse<List<DailyReportResponse>>> GetDailyReport()
    {
        var result = await _db.Transactions
            .GroupBy(t => DateOnly.FromDateTime(t.Date))
            .Select(g => new DailyReportResponse
            {
                Date = g.Key,
                TotalTransactions = g.Count(),
                TotalRevenue = g.Sum(x => x.TotalPrice)
            })
            .OrderByDescending(x => x.Date)
            .ToListAsync();

        return ApiResponse<List<DailyReportResponse>>.Ok(result);
    }

    // MONTHLY REPORT
    public async Task<ApiResponse<List<MonthlyReportResponse>>> GetMonthlyReport()
    {
        var result = await _db.Transactions
            .GroupBy(t => new { t.Date.Year, t.Date.Month })
            .Select(g => new MonthlyReportResponse
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                TotalTransactions = g.Count(),
                TotalRevenue = g.Sum(x => x.TotalPrice)
            })
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month)
            .ToListAsync();

        return ApiResponse<List<MonthlyReportResponse>>.Ok(result);
    }

    // SUMMARY REPORT
    public async Task<ApiResponse<SummaryReportResponse>> GetSummaryReport()
    {
        int totalTransactions = await _db.Transactions.CountAsync();
        decimal totalRevenue = await _db.Transactions.SumAsync(t => t.TotalPrice);

        var topProducts = await _db.TransactionItems
            .Include(i => i.Product)
            .GroupBy(i => i.Product.Name)
            .Select(g => new TopProductItem
            {
                ProductName = g.Key,
                TotalQty = g.Sum(x => x.Qty)
            })
            .OrderByDescending(x => x.TotalQty)
            .Take(5)
            .ToListAsync();

        var result = new SummaryReportResponse
        {
            TotalTransactions = totalTransactions,
            TotalRevenue = totalRevenue,
            TopProducts = topProducts
        };

        return ApiResponse<SummaryReportResponse>.Ok(result);
    }
}
