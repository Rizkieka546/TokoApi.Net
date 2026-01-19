namespace TokoApi.DTOs.Responses;

public class TransactionResponse
{
    public Guid Id { get; set; }
    public string KasirName { get; set; } = null!;
    public decimal TotalPrice { get; set; }
    public string PaymentMethod { get; set; } = null!;
    public DateTime Date { get; set; }
}