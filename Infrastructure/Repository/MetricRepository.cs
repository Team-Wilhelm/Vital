using System.Data;
using System.Runtime.InteropServices.JavaScript;
using Dapper;
using Infrastructure.Repository.Interface;
using Models;
using Models.Days;
using Models.Dto.Cycle;
using Models.Dto.Metrics;
using Models.Pagination;
using Models.Util;

namespace Infrastructure.Repository;

public class MetricRepository : IMetricRepository
{
    private readonly IDbConnection _db;
    private readonly ICalendarDayRepository _calendarDayRepository;

    public MetricRepository(IDbConnection db, ICalendarDayRepository calendarDayRepository)
    {
        _db = db;
        _calendarDayRepository = calendarDayRepository;
    }
    
    public async Task<ICollection<CalendarDayMetric>> Get(Guid userId, DateTimeOffset date)
    {
        var calendarDay = await _calendarDayRepository.GetByDate(userId, date);
        var sql = $@"SELECT
                CDM.""Id"" as {nameof(CalendarDayMetric.Id)},
                CDM.""CalendarDayId"" as {nameof(CalendarDayMetric.CalendarDayId)},
                CDM.""MetricsId"" as {nameof(CalendarDayMetric.MetricsId)},
                CDM.""MetricValueId"" as {nameof(CalendarDayMetric.MetricValueId)},
                ""Metrics"".""Id"" as {nameof(CalendarDayMetric.Metrics.Id)},
                ""Metrics"".""Name"" as {nameof(CalendarDayMetric.Metrics.Name)},
                MV.""Id"" as {nameof(CalendarDayMetric.MetricValue.Id)},
                MV.""Name"" as {nameof(CalendarDayMetric.MetricValue.Name)},
                MV.""MetricsId"" as {nameof(CalendarDayMetric.MetricValue.MetricsId)}
                FROM ""CalendarDayMetric"" CDM
                    INNER JOIN ""MetricValue"" MV ON CDM.""MetricValueId"" = MV.""Id""
                    INNER JOIN ""Metrics"" on ""Metrics"".""Id"" = CDM.""MetricsId""    
                WHERE CDM.""CalendarDayId""=@calendarDayId";
        if (calendarDay is null)
        {
            return new List<CalendarDayMetric>();
        }
        
        // The result: Id, CalendarDayId, MetricsId, MetricValueId, Id, Name, Id, Name, MetricsId
        var metrics = await _db.QueryAsync<CalendarDayMetric, Metrics, MetricValue, CalendarDayMetric>(
            sql,
            (calendarDayMetrics, metrics, metricValue) =>
            {
                metrics.Values = new List<MetricValue>(){metricValue};
                calendarDayMetrics.Metrics = metrics;
                calendarDayMetrics.MetricValue = metricValue;
                return calendarDayMetrics;
            }, splitOn: "Id, Id", 
            
            param: new { calendarDayId = calendarDay.Id });
        return metrics.ToList();
    }

    public async Task UploadMetricForADay(Guid calendarDayId, List<MetricRegisterMetricDto> metrics)
    {
        // Delete all metrics for the day, if there are any
        var sql = @"DELETE FROM ""CalendarDayMetric"" WHERE ""CalendarDayId""=@calendarDayId";
        await _db.ExecuteAsync(sql, new { calendarDayId });
        
        // Insert new metrics for the day
        sql = @"INSERT INTO ""CalendarDayMetric"" (""Id"",""CalendarDayId"", ""MetricsId"", ""MetricValueId"") VALUES (@Id, @calendarDayId, @metricsId, @metricValueId)";
        foreach (var metricsDto in metrics)
        {
            await _db.ExecuteAsync(sql, new { Id = Guid.NewGuid(), calendarDayId, metricsId = metricsDto.MetricsId, metricValueId = metricsDto.MetricValueId });
        }
    }
}
