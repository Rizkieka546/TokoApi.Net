namespace TokoApi.DTOs.Responses;

public class ProductResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string CategoryName { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public string Description { get; set; } = "";
    public bool IsActive { get; set; }
}
