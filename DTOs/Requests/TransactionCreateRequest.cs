namespace TokoApi.DTOs.Requests;

public class TransactionCreateRequest
{
    public List<TransactionItemRequest> Items { get; set; } = new List<T>();
    public string PaymentMethod { get; set; } = null!;
    public decimal PaidAmount { get; set; }
}
