using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TokoApi.Services;

namespace TokoApi.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize(Roles = "admin")]
public class ReportController : ControllerBase
{
    private readonly ReportService _service;

    public ReportController(ReportService service)
    {
        _service = service;
    }

    [HttpGet("daily")]
    public async Task<IActionResult> Daily()
    {
        var result = await _service.GetDailyReport();
        return Ok(result);
    }

    [HttpGet("monthly")]
    public async Task<IActionResult> Monthly()
    {
        var result = await _service.GetMonthlyReport();
        return Ok(result);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> Summary()
    {
        var result = await _service.GetSummaryReport();
        return Ok(result);
    }
}
