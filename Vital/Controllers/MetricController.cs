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

    [HttpGet("calendar")]
    public async Task<IActionResult> GetMetricsForCalendarDays([FromQuery] DateTimeOffset fromDate, [FromQuery]
        DateTimeOffset toDate)
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
    /// <param name="date"></param>
    [HttpGet("{date}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<CalendarDayMetric>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync([FromRoute] DateTimeOffset date)
    {
        var userId = _currentContext.UserId!.Value;
        var list = await _metricService.Get(userId, date);
        return Ok(list);
    }

    /// <summary>
    /// Deletes any existing metrics for the day and uploads the new metrics.
    /// </summary>
    /// <param name="metrics"></param>
    /// <param name="dateTimeOffsetString"></param>
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CalendarDayDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SaveMetricsAsync([FromBody] List<MetricRegisterMetricDto> metrics, [FromQuery] string dateTimeOffsetString)
    {
        var dateTimeOffset = DateTimeOffset.Parse(dateTimeOffsetString);
        //var userId = _currentContext.UserId!.Value; //TODO: Uncomment this line and enable authentication 
        var userId = Guid.Parse("adfead4c-823b-41e5-9c7e-c84aa04192a4");
        var calendarDayDto = await _metricService.SaveMetrics(userId, metrics, dateTimeOffset);
        return Ok(calendarDayDto);
    }
}
