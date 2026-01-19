namespace TokoApi.DTOs.Responses;

public class SummaryReportResponse
{
    public int TotalTransactions { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<TopProductItem> TopProducts { get; set; } 
        = new List<TopProductItem>();
}

public class TopProductItem
{
    public string ProductName { get; set; } = null!;
    public int TotalQty { get; set; }
}
