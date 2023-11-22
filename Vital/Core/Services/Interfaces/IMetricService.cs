using Models.Dto.Cycle;
using Models.Dto.Metrics;
using Models.Util;

namespace Vital.Core.Services.Interfaces;

public interface IMetricService
{
    Task<ICollection<CalendarDayMetric>> Get(Guid userId, DateTimeOffset date);
    Task<CalendarDayDto> UploadMetricForADay(Guid userId, List<MetricRegisterMetricDto> metricsDto, DateTimeOffset dateTimeOffset);
}
