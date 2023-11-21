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

    public MetricRepository(IDbConnection db)
    {
        _db = db;
    }
    
    public async Task<ICollection<CalendarDayMetric>> Get(Guid userId, DateTimeOffset date)
    {
        var calendarDayId = await GetCalendarDayId(userId, date);
        
        var sql = @"SELECT * FROM ""Metrics"" 
                    INNER JOIN ""MetricValue"" ON ""Metrics"".""Id"" = ""MetricValue"".""MetricsId""
                    INNER JOIN public.""CalendarDayMetric"" CDM on ""Metrics"".""Id"" = CDM.""MetricsId""    
                    WHERE ""CalendarDayId""=@calendarDayId";
        
        var metrics = await _db.QueryAsync<CalendarDayMetric>(sql, new
        {
            calendarDayId
        });
        return metrics.ToList();
    }

    public async Task<Guid> GetCalendarDayId(Guid userId, DateTimeOffset date)
    {
        var sql = @"SELECT ""Id"" FROM ""CalendarDay"" WHERE ""UserId""=@userId AND ""Date""=@date";
        var calendarDayId = await _db.QuerySingleOrDefaultAsync<Guid>(sql, new { userId, date });
        return calendarDayId;
    }

    public async Task UploadMetricForADay(Guid calendarDayId, List<MetricsDto> metricsDtoList)
    {
        // Delete all metrics for the day, if there are any
        var sql = @"DELETE FROM ""CalendarDayMetrics"" WHERE ""CalendarDaysId""=@calendarDayId";
        await _db.ExecuteAsync(sql, new { calendarDayId });
        
        // Insert new metrics for the day
        sql = @"INSERT INTO ""CalendarDayMetrics"" (""CalendarDaysId"", ""MetricsId"") VALUES (@calendarDayId, @metricsId)";
        foreach (var metricsDto in metricsDtoList)
        {
            await _db.ExecuteAsync(sql, new { calendarDayId, metricsId = metricsDto.Id });
        }
    }
}
