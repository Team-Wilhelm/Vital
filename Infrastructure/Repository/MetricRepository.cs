using System.Data;
using Dapper;
using Infrastructure.Adapters;
using Infrastructure.Repository.Interface;
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
            groupedMetric.Values = g.Select(p => p.Values.FirstOrDefault()).Where(p => p != null).ToList() ??
                                   new List<MetricValue>();
            return groupedMetric;
        }).ToList();
    }

    public async Task<IEnumerable<CalendarDayAdapter>> GetMetricsForCalendarDays(Guid userId, DateTimeOffset fromDate,
        DateTimeOffset toDate)
    {
        var sql = $@"SELECT 
                ""CalendarDay"".""Id"" as {nameof(CalendarDayAdapter.CalendarDayId)},
                ""CalendarDay"".""Date"" as {nameof(CalendarDayAdapter.Date)},
                ""CalendarDay"".""UserId"" as {nameof(CalendarDayAdapter.UserId)},
                ""CalendarDay"".""State"" as {nameof(CalendarDayAdapter.State)},
                ""CalendarDay"".""CycleId"" as {nameof(CalendarDayAdapter.CycleId)},
                ""CalendarDay"".""IsPeriod"" as {nameof(CalendarDayAdapter.IsPeriod)},
                CDM.""Id"" as {nameof(CalendarDayAdapter.CalendarDayMetricId)},
                CDM.""CreatedAt"" as {nameof(CalendarDayAdapter.CreatedAt)},
                M.""Id"" as {nameof(CalendarDayAdapter.MetricsId)},
                M.""Name"" as {nameof(CalendarDayAdapter.MetricName)},
                MV.""Id"" as {nameof(CalendarDayAdapter.MetricValueId)},
                MV.""Name"" as {nameof(CalendarDayAdapter.MetricValueName)}        
                FROM ""CalendarDay""
                JOIN ""CalendarDayMetric"" CDM on ""CalendarDay"".""Id"" = CDM.""CalendarDayId""
                JOIN ""MetricValue"" MV on CDM.""MetricValueId"" = MV.""Id""
                JOIN ""Metrics"" M on M.""Id"" = CDM.""MetricsId""
        WHERE CAST(""Date"" AS DATE) >= CAST(@fromDate AS DATE) AND CAST(""Date"" AS DATE) <= CAST(@toDate AS DATE) 
          AND ""UserId"" = @userId";

        var result = await _db.QueryAsync<CalendarDayAdapter>(sql, new
        {
            userId, fromDate, toDate
        });
        return result;
    }

    //TODO parse date correctly
    public async Task<IEnumerable<DateTimeOffset>> GetPeriodDays(Guid userId, DateTimeOffset fromDate, DateTimeOffset toDate)
    {
        var sql = $@"SELECT
    CAST(CD.""Date"" AS TIMESTAMP WITH TIME ZONE) as ""Date""
    FROM ""CalendarDay"" CD
    WHERE CAST(""Date"" AS DATE) >= CAST(@fromDate AS DATE) AND CAST(""Date"" AS DATE) <= CAST(@toDate AS DATE)
      AND ""UserId"" = @userId AND ""IsPeriod"" = true
    ";
        var result = await _db.QueryAsync<string>(sql, new { userId, fromDate, toDate });
        var parsedDates = result.Select(dateString => DateTimeOffset.Parse(dateString));
        return parsedDates;
    }
    
    public async Task<ICollection<CalendarDayMetric>> Get(Guid userId, DateTimeOffset date)
    {
        // Because the data in the database is stored in UTC, but we want to retrieve it in the user's timezone,
        // we need to convert the date to UTC and add/subtract the offset + 24 hours to retrieve the correct data for the user's timezone.
        // For example, if the user is trying to retrieve the data for 2023-11-27+01:00, we need to get anything between 2023-11-26T23:00:00Z and 2023-11-27T22:59:59Z
        var start = date.UtcDateTime;
        var end = date.UtcDateTime.AddDays(1); 
        var sql = $@"SELECT
                CDM.*,
                ""Metrics"".*,
                MV.*
                FROM ""CalendarDayMetric"" CDM
                    LEFT JOIN ""MetricValue"" MV ON CDM.""MetricValueId"" = MV.""Id""
                    LEFT JOIN ""Metrics"" on ""Metrics"".""Id"" = CDM.""MetricsId""    
                WHERE ""CreatedAt"" >= @start AND ""CreatedAt"" < @end AND ""CalendarDayId"" IN (
                    SELECT ""Id"" FROM ""CalendarDay"" WHERE ""UserId"" = @userId);";

        // The result: Id, CalendarDayId, CreatedAt, MetricsId, MetricValueId, Id, Name, Id, Name, MetricsId
        var metrics = await _db.QueryAsync<CalendarDayMetric, Metrics, MetricValue, CalendarDayMetric>(
            sql,
            (calendarDayMetrics, metrics, metricValue) =>
            {
                metrics.Values = new List<MetricValue>() { metricValue };
                calendarDayMetrics.Metrics = metrics;
                calendarDayMetrics.MetricValue = metricValue;
                return calendarDayMetrics;
            }, splitOn: "Id, Id",
            param: new { userId, start, end });
        return metrics.ToList();
    }

    public async Task SaveMetrics(Guid calendarDayId, List<MetricRegisterMetricDto> metrics)
    {
        // Insert new metrics for the day
        var sql = @"INSERT INTO ""CalendarDayMetric"" (""Id"",""CalendarDayId"", ""MetricsId"", ""MetricValueId"", ""CreatedAt"") VALUES (@Id, @calendarDayId, @metricsId, @metricValueId, @createdAt)";
        foreach (var metricsDto in metrics)
        {
            await _db.ExecuteAsync(sql,
                new
                {
                    Id = Guid.NewGuid(), calendarDayId, metricsId = metricsDto.MetricsId, createdAt = metricsDto.CreatedAt,
                    metricValueId = metricsDto.MetricValueId ?? (object)DBNull.Value
                });
        }
    }
    
    public async Task<Dictionary<Guid, string>> GetMetricNamesByIds(List<Guid> metricIds)
    {
        var sql = @"SELECT ""Id"", ""Name"" FROM ""Metrics"" WHERE ""Id"" = ANY(@metricIds)";
        var result = await _db.QueryAsync<Metrics>(sql, new { metricIds });
        return result.ToDictionary(m => m.Id, m => m.Name);
    }
   
    public async Task DeleteMetricEntry(Guid calendarDayMetricId)
    {
        var sql = @"DELETE FROM ""CalendarDayMetric"" WHERE ""Id"" = @calendarDayMetricId";
        await _db.ExecuteAsync(sql, new { calendarDayMetricId });
    }

    public async Task<List<Metrics>> GetMetricsForCalendarDayById(Guid calendarDayId)
    {
        var sql = @"SELECT M.*, MV.* 
                    FROM ""Metrics"" M
                    LEFT JOIN public.""MetricValue"" MV on M.""Id"" = MV.""MetricsId""
                    WHERE M.""Id"" IN (
                        SELECT ""MetricsId"" FROM ""CalendarDayMetric"" WHERE ""CalendarDayId"" = @calendarDayId
                    )";
        var list = await _db.QueryAsync<Metrics, MetricValue, Metrics>(sql, (metrics, value) =>
        {
            metrics.Values.Add(value);
            return metrics;
        }, new { calendarDayId });
        
        // Group the metrics by their id and select the first one, since all of the elements in the group are the same, to prevent duplicates
        // Then, combine all the values from the group and add them to the metric
        return list
            .GroupBy(m => m.Id) 
            .Select(g => 
        {
            var groupedMetric = g.First();
            groupedMetric.Values = g.Select(p => p.Values.FirstOrDefault()).Where(p => p != null).ToList();
            return groupedMetric;
        }).ToList();
    }

    public async Task<Guid> GetCalendarDayIdByCalendarDayMetricId(Guid calendarDayMetricId)
    {
        var sql = @"SELECT ""CalendarDayId"" FROM ""CalendarDayMetric"" WHERE ""Id"" = @calendarDayMetricId";
        var result = await _db.QuerySingleOrDefaultAsync<Guid>(sql, new { calendarDayMetricId });
        return result;
    }

    public async Task<bool> CheckIfMetricsExist(List<MetricRegisterMetricDto> metrics)
    {
        // Check if the metric passed is valid
        var metricIds = metrics.Select(m => m.MetricsId).Distinct().ToList();
        var sql = @"SELECT ""Id"" FROM ""Metrics"" WHERE ""Id"" = ANY(@metricIds)";
        var validMetricsIds = await _db.QueryAsync<Guid>(sql, new { metricIds });
        if (validMetricsIds.Count() != metricIds.Count)
        {
            throw new BadRequestException("The metric you are trying to log does not exist.");
        }

        // Check if the metric value passed is valid
        var metricValuesIds = metrics.Select(m => m.MetricValueId).Distinct().ToList();
        metricValuesIds.RemoveAll(m => m == null);
        if (metricValuesIds.Count > 0)
        {
            sql = @"SELECT ""Id"" FROM ""MetricValue"" WHERE ""Id"" = ANY(@metricValuesIds)";
            var validMetricValuesIds = await _db.QueryAsync<Guid>(sql, new { metricValuesIds });
            if (validMetricValuesIds.Count() != metricValuesIds.Count)
            {
                throw new BadRequestException("The metric value you are trying to log does not exist.");
            }
        }
        return true;
    }
}
