using Microsoft.AspNetCore.Http;

namespace TokoApi.DTOs.Requests;

public class ProductCreateRequest
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public Guid CategoryId { get; set; }
    public string Description { get; set; } = "";
    public IFormFile Image { get; set; } = null!;
}
