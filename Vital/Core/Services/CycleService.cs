using AutoMapper;
using Infrastructure.Repository.Interface;
using Models;
using Models.Dto;
using Models.Pagination;
using Vital.Core.Context;
using Vital.Core.Services.Interfaces;
using Vital.Models.Exception;

namespace Vital.Core.Services;

public class CycleService : ICycleService
{
    private readonly ICycleRepository _cycleRepository;
    private readonly IMapper _mapper;
    private readonly CurrentContext _currentContext;

    public CycleService(ICycleRepository cycleRepository, IMapper mapper, CurrentContext currentContext)
    {
        _cycleRepository = cycleRepository;
        _mapper = mapper;
        _currentContext = currentContext;
    }

    public async Task<PaginatedList<Cycle>> Get(Paginator paginator)
    {
        return await _cycleRepository.Get(paginator);
    }

    public async Task<Cycle?> GetById(Guid id)
    {
        return await _cycleRepository.GetById(id);
    }

    public async Task<Cycle> Create(CreateCycleDto dto)
    {
        var cycle = _mapper.Map<Cycle>(dto);
        cycle.Id = Guid.NewGuid();
        cycle.UserId = _currentContext.UserId!.Value;
        await _cycleRepository.Create(cycle);
        
        return cycle;
    }

    public async Task<Cycle> Update(Guid id, UpdateCycleDto dto)
    {
        var cycle = await _cycleRepository.GetById(id);
        if (cycle is null)
        {
            throw new NotFoundException();
        }
        
        _mapper.Map(dto, cycle);
        await _cycleRepository.Update(cycle);
        
        return cycle;
    }
}
