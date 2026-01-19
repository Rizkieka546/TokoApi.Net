using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TokoApi.DTOs.Requests;
using TokoApi.Services;

namespace TokoApi.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "admin")]
public class UserController : ControllerBase
{
    private readonly UserService _service;

    public UserController(UserService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAll();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserCreateRequest request)
    {
        var result = await _service.Create(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UserUpdateRequest request)
    {
        var result = await _service.Update(id, request);
        return Ok(result);
    }

    [HttpPatch("{id}/reset-password")]
    public async Task<IActionResult> ResetPassword(Guid id, UserResetPasswordRequest request)
    {
        var result = await _service.ResetPassword(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.Delete(id);
        return Ok(result);
    }
}
