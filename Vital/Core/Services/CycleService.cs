using Infrastructure.Repository.Interface;
using Models;
using Models.Pagination;
using Vital.Core.Services.Interfaces;

namespace Vital.Core.Services;

public class CycleService : ICycleService
{
    private readonly ICycleRepository _cycleRepository;

    public CycleService(ICycleRepository cycleRepository)
    {
        _cycleRepository = cycleRepository;
    }

    public async Task<PaginatedList<Cycle>> Get(Paginator paginator)
    {
        return await _cycleRepository.Get(paginator);
    }

    public async Task<Cycle?> GetById(Guid id)
    {
        return await _cycleRepository.GetById(id);
    }
}
