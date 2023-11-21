using Models;
using Models.Pagination;

namespace Vital.Core.Services.Interfaces;

public interface IMetricService
{
    Task<PaginatedList<Metrics>> Get(Guid userId, DateTimeOffset date, Paginator paginator);
}
