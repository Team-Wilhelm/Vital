using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Dto;
using Models.Pagination;
using Vital.Core.Services.Interfaces;
using Vital.Extension.Mapping;
using Vital.Models.Exception;

namespace Vital.Controllers;

/// <summary>
/// Controller responsible for accessing Cycle data.
/// </summary>
[Authorize]
public class CycleController : BaseController
{
    private readonly ICycleService _cycleService;
    private readonly IMapper _mapper;

    public CycleController(ICycleService cycleService, IMapper mapper)
    {
        _cycleService = cycleService;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves a paginated list of Cycle objects.
    /// </summary>
    /// <param name="paginator">An object of type Paginator which contains parameters for pagination.</param>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<Cycle>))]
    public async Task<IActionResult> GetAll([FromQuery] Paginator paginator)
    {
        var list = await _cycleService.Get(paginator);

        return Ok(_mapper.MapPaginatedList<Cycle, CycleDto>(list));
    }

    /// <summary>
    /// Retrieves a Cycle object by its ID.
    /// </summary>
    /// <param name="id">The unique Guid identifier of the Cycle object.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Cycle))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var cycle = await _cycleService.GetById(id);
        if (cycle is null)
        {
            throw new NotFoundException();
        }

        return Ok(cycle);
    }
    
    /// <summary>
    /// Creates a new Cycle object with the specified details.
    /// </summary>
    /// <param name="dto">A CreateCycleDto object containing the details for the new Cycle object.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Cycle))]
    public async Task<IActionResult> Create([FromBody] CreateCycleDto dto)
    {
        var cycle = await _cycleService.Create(dto);

        return Created("", cycle);
    }

    /// <summary>
    /// Updates an existing Cycle object with new values.
    /// </summary>
    /// <param name="id">The unique identifier of the Cycle object to be updated.</param>
    /// <param name="dto">A UpdateCycleDto object containing the updated values.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Cycle))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCycleDto dto)
    {
        var cycle = await _cycleService.Update(id, dto);

        return Ok(cycle);
    }
    
}
