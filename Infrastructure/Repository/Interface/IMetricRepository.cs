using Models;
using Models.Pagination;

namespace Infrastructure.Repository.Interface;

public interface IMetricRepository
{
    Task<PaginatedList<Metrics>> Get(Guid userId, DateTimeOffset date, Paginator paginator);
}
