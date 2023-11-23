using System.Collections;
using System.Data;
using Dapper;
using Infrastructure.Adapters;
using Infrastructure.Repository.Interface;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Models;
using Models.Days;
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

    public async Task<List<Metrics>> GetAllMetrics()
    {
        var sql = @"SELECT M.*, MV.* 
                    FROM ""Metrics"" M
                    LEFT JOIN public.""MetricValue"" MV on M.""Id"" = MV.""MetricsId""";
        var list = await _db.QueryAsync<Metrics, MetricValue, Metrics>(sql,
            (metrics, value) =>
            {
                metrics.Values.Add(value);
                return metrics;
            });
        return list.GroupBy(m => m.Id).Select(g =>
        {
            var groupedMetric = g.First();
            groupedMetric.Values = g.Select(p => p.Values.FirstOrDefault()).Where(p => p != null).ToList() ?? new List<MetricValue>();
            return groupedMetric;
        }).ToList();
    }
    
    public async Task<IEnumerable<CalendarDayAdapter>> GetMetricsForCalendarDays(Guid userId, DateTimeOffset fromDate, DateTimeOffset toDate)
    {
        var sql = $@"SELECT 
                ""CalendarDay"".""Id"" as {nameof(CalendarDayAdapter.CalendarDayId)},
                ""CalendarDay"".""Date"" as {nameof(CalendarDayAdapter.Date)},
                ""CalendarDay"".""UserId"" as {nameof(CalendarDayAdapter.UserId)},
                ""CalendarDay"".""State"" as {nameof(CalendarDayAdapter.State)},
                ""CalendarDay"".""CycleId"" as {nameof(CalendarDayAdapter.CycleId)},
                ""CalendarDay"".""IsPeriodDay"" as {nameof(CalendarDayAdapter.IsPeriodDay)},
                CDM.""Id"" as {nameof(CalendarDayAdapter.CalendarDayMetricId)},
                M.""Id"" as {nameof(CalendarDayAdapter.MetricsId)},
                M.""Name"" as {nameof(CalendarDayAdapter.MetricName)},
                MV.""Id"" as {nameof(CalendarDayAdapter.MetricValueId)},
                MV.""Name"" as {nameof(CalendarDayAdapter.MetricValueName)}        
                FROM ""CalendarDay""
                JOIN ""CalendarDayMetric"" CDM on ""CalendarDay"".""Id"" = CDM.""CalendarDayId""
                JOIN ""MetricValue"" MV on CDM.""MetricValueId"" = MV.""Id""
                JOIN ""Metrics"" M on M.""Id"" = CDM.""MetricsId""
        WHERE CAST(""Date"" AS DATE) >= @from AND CAST(""Date"" AS DATE) <= @to
                AND ""UserId"" = @userId";

        var result = await _db.QueryAsync<CalendarDayAdapter>(sql, new
        {
            userId, from = fromDate, to = toDate
        });
        
        return result;
    }
    
    public async Task<ICollection<CalendarDayMetric>> Get(Guid userId, DateTimeOffset date)
    {
        var calendarDay = await _calendarDayRepository.GetByDate(userId, date);
        var sql = $@"SELECT
                CDM.*,
                ""Metrics"".*,
                MV.*
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
