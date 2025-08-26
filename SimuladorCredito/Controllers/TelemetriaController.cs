using Microsoft.AspNetCore.Mvc;
using SimuladorCredito.Services;

[ApiController]
[Route("[controller]")]
public class TelemetriaController : ControllerBase
{
    private readonly TelemetryService _telemetryService;

    public TelemetriaController(TelemetryService telemetryService)
    {
        _telemetryService = telemetryService;
    }

    [HttpGet("telemetria")]
    public IActionResult GetTelemetry()
    {
        var result = new
        {
            Uptime = _telemetryService.GetUptime().TotalSeconds,
            RequestCount = _telemetryService.GetRequestCount(),
            StartTime = _telemetryService.GetStartTime(),
            Requests = _telemetryService.GetRequests()
                .OrderByDescending(r => r.Timestamp)
                .Take(100) // Retorna os 100 últimos registros
        };
        return Ok(result);
    }


}
