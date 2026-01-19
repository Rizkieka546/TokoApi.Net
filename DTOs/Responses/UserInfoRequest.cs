
namespace TokoApi.DTOs.Responses;

public class UserInfoResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = "kasir";
}
