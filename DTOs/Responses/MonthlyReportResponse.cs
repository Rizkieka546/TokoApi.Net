namespace TokoApi.DTOs.Responses;

public class MonthlyReportResponse
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int TotalTransactions { get; set; }
    public decimal TotalRevenue { get; set; }
}
