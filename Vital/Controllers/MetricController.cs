using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Dto.Cycle;
using Models.Dto.Metrics;
using Models.Pagination;
using Models.Util;
using Vital.Core.Context;
using Vital.Core.Services;
using Vital.Core.Services.Interfaces;
using Metrics = Microsoft.Identity.Client.Metrics;

namespace Vital.Controllers;

[Authorize]
public class MetricController : BaseController
{
    // TODO? Add interface
    private readonly IMetricService _metricService;
    private readonly CurrentContext _currentContext;
    
    public MetricController(IMetricService metricService, CurrentContext currentContext)
    {
        _metricService = metricService;
        _currentContext = currentContext;
    }
    
    [HttpGet("{date}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<CalendarDayMetric>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] string date)
    {
        var parsedDate = DateTimeOffset.Parse(date);
        var userId = _currentContext.UserId!.Value;
        
        var list = await _metricService.Get(userId, parsedDate);
        return Ok(list);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CalendarDayDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadMetricForADay([FromBody] List<MetricsDto> metricsDto, [FromQuery] string dateTimeOffsetString)
    {
        var dateTimeOffset = DateTimeOffset.Parse(dateTimeOffsetString);
        var userId = _currentContext.UserId!.Value;
        var calendarDayDto = await _metricService.UploadMetricForADay(userId, metricsDto, dateTimeOffset);
        return Ok(calendarDayDto);
    }
}
