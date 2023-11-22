using System.Data;
using Dapper;
using Infrastructure.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Models;
using Models.Dto.Metrics;
using Models.Util;
using Vital.Models.Exception;

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
                metrics.Values = new List<MetricValue>() { metricValue };
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
        
        // Check if the metric passed is valid
        var metricsIds = metrics.Select(m => m.MetricsId).Distinct().ToList();
        sql = @"SELECT ""Id"" FROM ""Metrics"" WHERE ""Id"" = ANY(@metricsIds)";
        var validMetricsIds = await _db.QueryAsync<Guid>(sql, new { metricsIds });
        if (validMetricsIds.Count() != metricsIds.Count)
        {
            throw new BadRequestException("The metric you are trying to log does not exist.");
        }
        
        // Check if the metric value passed is valid
        var metricValuesIds = metrics.Select(m => m.MetricValueId).Distinct().ToList();
        sql = @"SELECT ""Id"" FROM ""MetricValue"" WHERE ""Id"" = ANY(@metricValuesIds)";
        var validMetricValuesIds = await _db.QueryAsync<Guid>(sql, new { metricValuesIds });
        if (validMetricValuesIds.Count() != metricValuesIds.Count)
        {
            throw new BadRequestException("The metric value you are trying to log does not exist.");
        }

        // Insert new metrics for the day
        sql = @"INSERT INTO ""CalendarDayMetric"" (""Id"",""CalendarDayId"", ""MetricsId"", ""MetricValueId"") VALUES (@Id, @calendarDayId, @metricsId, @metricValueId)";
        foreach (var metricsDto in metrics)
        {
            await _db.ExecuteAsync(sql, new { Id = Guid.NewGuid(), calendarDayId, metricsId = metricsDto.MetricsId, metricValueId = metricsDto.MetricValueId });
        }
    }
}
