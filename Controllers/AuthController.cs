using Microsoft.AspNetCore.Mvc;
using TokoApi.DTOs.Requests;
using TokoApi.Responses;
using TokoApi.Services;

namespace TokoApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _service;

    public AuthController(AuthService service)
    {
        _service = service;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _service.Login(request, Response);
        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _service.Logout(Response);
        return Ok(ApiResponse<string>.Ok(null, "Logout berhasil"));
    }
}
