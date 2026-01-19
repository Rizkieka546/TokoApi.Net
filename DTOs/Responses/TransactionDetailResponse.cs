namespace TokoApi.DTOs.Responses;

public class TransactionDetailResponse
{
    public Guid Id { get; set; }
    public string KasirName { get; set; } = null!;
    public decimal TotalPrice { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal ChangeAmount { get; set; }
    public string PaymentMethod { get; set; } = null!;
    public DateTime Date { get; set; }

    public List<TransactionItemDetail> Items { get; set; } 
        = new List<TransactionItemDetail>();
}

public class TransactionItemDetail
{
    public string ProductName { get; set; } = null!;
    public int Qty { get; set; }
    public decimal PriceSnapshot { get; set; }
}
