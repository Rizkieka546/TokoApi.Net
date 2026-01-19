using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TokoApi.DTOs.Requests;
using TokoApi.Responses;
using TokoApi.Services;

namespace TokoApi.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize(Roles = "admin")]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _service;

    public CategoryController(CategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        var result = await _service.GetAll(page, pageSize, search);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryRequest request)
    {
        var result = await _service.Create(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, CategoryRequest request)
    {
        var result = await _service.Update(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.Delete(id);
        return Ok(result);
    }
}
