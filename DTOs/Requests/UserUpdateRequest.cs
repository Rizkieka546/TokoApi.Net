namespace TokoApi.DTOs.Requests;

public class UserUpdateRequest
{
    public string Username { get; set; } = null!;
    public string Role { get; set; } = null!;
}
