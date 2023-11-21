using Infrastructure.Repository;
using Infrastructure.Repository.Interface;
using Models;
using Models.Pagination;
using Vital.Core.Services.Interfaces;

namespace Vital.Core.Services;

public class MetricService : IMetricService
{
    //TODO: Add interface
    private readonly IMetricRepository _metricRepository;
    
    public MetricService(IMetricRepository metricRepository)
    {
        _metricRepository = metricRepository;
    }

    public async Task<PaginatedList<Metrics>> Get(Guid userId, DateTimeOffset date, Paginator paginator)
    {
        var list = await _metricRepository.Get(userId, date, paginator);
        return list;
    }
}
