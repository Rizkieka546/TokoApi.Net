namespace TokoApi.DTOs.Responses;

public class DailyReportResponse
{
    public DateOnly Date { get; set; }
    public int TotalTransactions { get; set; }
    public decimal TotalRevenue { get; set; }
}
