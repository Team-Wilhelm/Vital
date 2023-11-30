using System.Runtime.InteropServices.JavaScript;
using AutoMapper;
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
    private readonly IMetricService _metricService;
    private readonly CurrentContext _currentContext;
    private readonly IMapper _mapper;

    public MetricController(IMetricService metricService, CurrentContext currentContext, IMapper mapper)
    {
        _metricService = metricService;
        _currentContext = currentContext;
        _mapper = mapper;
    }

    [AllowAnonymous]
    [HttpGet("values")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllValues()
    {
        var list = await _metricService.GetAllMetrics();

        return Ok(_mapper.Map<List<MetricsDto>>(list));
    }

    /// <summary>
    /// Gets the metrics for an interval of days for the current user.
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    [HttpGet("calendar")]
    public async Task<IActionResult> GetMetricsForCalendarDays([FromQuery] DateTimeOffset fromDate,
        [FromQuery] DateTimeOffset toDate)
    {
        if (toDate < fromDate)
        {
            throw new Exception("To date can't be before from date.");
        }

        var list = await _metricService.GetMetricsForCalendarDays(_currentContext.UserId!.Value, fromDate, toDate);
        return Ok(list);
    }

    /// <summary>
    /// Gets the metrics for a day for the current user (based on the token).
    /// </summary>
    /// <param name="dateString">The date string in YYYY-MM-DD+HH:mm or YYYY-MM-DD-HH:mm format.</param>
    [HttpGet("{dateString}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<CalendarDayMetric>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync([FromRoute] string dateString)
    {
        var date = DateTimeOffset.Parse(dateString);
        var userId = _currentContext.UserId!.Value;
        var list = await _metricService.Get(userId, date);
        return Ok(list);
    }

    /// <summary>
    /// Deletes any existing metrics for the day and uploads the new metrics.
    /// </summary>
    /// <param name="metrics"></param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SaveMetricsAsync([FromBody] List<MetricRegisterMetricDto> metrics)
    {
        var userId = _currentContext.UserId!.Value;
        await _metricService.SaveMetrics(userId, metrics);
        return Ok();
    }

    /// <summary>
    /// Gets the period days for the current user for a given period.
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <returns></returns>
    [HttpGet("period")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<DateTimeOffset>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPeriodDaysAsync([FromQuery] DateTimeOffset fromDate,
        [FromQuery] DateTimeOffset toDate)
    {
        var userId = _currentContext.UserId!.Value;
        var periodDays = await _metricService.GetPeriodDays(userId, fromDate, toDate);
        return Ok(periodDays);
    }
    
    /// <summary>
    /// Deletes a metric entry.
    /// </summary>
    /// <param name="calendarDayMetricId"></param>
    /// <returns></returns>
    [HttpDelete("{calendarDayMetricId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMetricEntryAsync([FromRoute] Guid calendarDayMetricId)
    {
        await _metricService.DeleteMetricEntry(calendarDayMetricId);
        return Ok();
    }
}
