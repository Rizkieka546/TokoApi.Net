namespace TokoApi.DTOs.Requests;

public class TransactionItemRequest
{
    public Guid ProductId { get; set; }
    public int Qty { get; set; }
}
