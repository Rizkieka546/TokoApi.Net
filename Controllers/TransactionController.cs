using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TokoApi.DTOs.Requests;
using TokoApi.Services;

namespace TokoApi.Controllers;

[ApiController]
[Route("api/transactions")]
public class TransactionController : ControllerBase
{
    private readonly TransactionService _service;

    public TransactionController(TransactionService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Roles = "kasir")]
    public async Task<IActionResult> Create(TransactionCreateRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _service.Create(userId, request);
        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = "admin,kasir")]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
    {
        var result = await _service.GetAll(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "admin,kasir")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetById(id);
        return Ok(result);
    }
}
