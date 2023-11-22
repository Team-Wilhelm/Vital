using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dto.Cycle;
using Models.Dto.Metrics;
using Models.Util;
using Vital.Core.Context;
using Vital.Core.Services.Interfaces;

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

    /// <summary>
    /// Gets the metrics for a day for the current user (based on the token).
    /// </summary>
    /// <param name="date"></param>
    [HttpGet("{date}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<CalendarDayMetric>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync([FromRoute] string date)
    {
        var parsedDate = DateTimeOffset.Parse(date);
        var userId = _currentContext.UserId!.Value;

        var list = await _metricService.Get(userId, parsedDate);
        return Ok(list);
    }

    /// <summary>
    /// Deletes any existing metrics for the day and uploads the new metrics.
    /// </summary>
    /// <param name="metrics"></param>
    /// <param name="dateTimeOffsetString"></param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CalendarDayDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadMetricForADayAsync([FromBody] List<MetricRegisterMetricDto> metrics, [FromQuery] string dateTimeOffsetString)
    {
        var dateTimeOffset = DateTimeOffset.Parse(dateTimeOffsetString);
        var userId = _currentContext.UserId!.Value;
        var calendarDayDto = await _metricService.UploadMetricForADay(userId, metrics, dateTimeOffset);
        return Ok(calendarDayDto);
    }
}
