﻿using Models.Dto.Metrics;
using Models.Util;

namespace Infrastructure.Repository.Interface;

public interface IMetricRepository
{
    Task<ICollection<CalendarDayMetric>> Get(Guid userId, DateTimeOffset date);
    Task UploadMetricForADay(Guid calendarDayId, List<MetricRegisterMetricDto> metrics);
}
