using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dto.Cycle;
using Models.Pagination;
using Vital.Core.Context;
using Vital.Core.Services;
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
    
    [HttpGet("{date}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<MetricsDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] string date, [FromQuery] Paginator paginator)
    {
        var parsedDate = DateTimeOffset.Parse(date);
        var userId = _currentContext.UserId!.Value;
        
        var list = await _metricService.Get(userId, parsedDate, paginator);
        return Ok(list);
    }
}
