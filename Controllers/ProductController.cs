using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TokoApi.DTOs.Requests;
using TokoApi.Services;

namespace TokoApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly ProductService _service;

    public ProductController(ProductService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll(
        int page = 1,
        int pageSize = 10,
        string? search = null,
        Guid? categoryId = null)
    {
        bool isAdmin = User.IsInRole("admin");
        var result = await _service.GetAll(page, pageSize, search, categoryId, isAdmin);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
    {
        var result = await _service.Create(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(Guid id, [FromForm] ProductUpdateRequest request)
    {
        var result = await _service.Update(id, request);
        return Ok(result);
    }

    [HttpPatch("{id}/toggle")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Toggle(Guid id)
    {
        var result = await _service.Toggle(id);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.Delete(id);
        return Ok(result);
    }
}
