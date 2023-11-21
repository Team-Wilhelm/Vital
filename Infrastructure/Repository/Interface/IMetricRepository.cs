using Models;
using Models.Days;
using Models.Dto.Cycle;
using Models.Dto.Metrics;
using Models.Pagination;
using Models.Util;

namespace Infrastructure.Repository.Interface;

public interface IMetricRepository
{
    Task<ICollection<CalendarDayMetric>> Get(Guid userId, DateTimeOffset date);
    Task UploadMetricForADay(Guid calendarDayId, List<MetricRegisterMetricDto> metrics);
}
