using Infrastructure.Adapters;
using Models;
using Models.Dto.Metrics;
using Models.Util;

namespace Infrastructure.Repository.Interface;

public interface IMetricRepository
{
    Task<List<Metrics>> GetAllMetrics();
    Task<IEnumerable<CalendarDayAdapter>> GetMetricsForCalendarDays(Guid userId, DateTimeOffset fromDate, DateTimeOffset toDate);
    Task<ICollection<CalendarDayMetric>> Get(Guid userId, DateTimeOffset date);
    Task SaveMetrics(Guid calendarDayId, List<MetricRegisterMetricDto> metrics);
    Task<IEnumerable<DateTimeOffset>> GetPeriodDays(Guid userId, DateTimeOffset fromDate, DateTimeOffset toDate);
    Task<Dictionary<Guid, string>> GetMetricNamesByIds(List<Guid> metricIds);
    Task DeleteMetricEntry(Guid calendarDayMetricId);
    Task<List<Metrics>> GetMetricsForCalendarDayById(Guid calendarDayId);
    Task<Guid> GetCalendarDayIdByCalendarDayMetricId(Guid calendarDayMetricId);
    Task<bool> CheckIfMetricsExist(List<MetricRegisterMetricDto> metrics);
}
