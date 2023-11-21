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
        if (calendarDay is null)
        {
            return new List<CalendarDayMetric>();
        }
        
        var sql = @"SELECT * FROM ""Metrics"" 
                    INNER JOIN ""MetricValue"" ON ""Metrics"".""Id"" = ""MetricValue"".""MetricsId""
                    INNER JOIN public.""CalendarDayMetric"" CDM on ""Metrics"".""Id"" = CDM.""MetricsId""    
                    WHERE ""CalendarDayId""=@calendarDayId";
        
        var metrics = await _db.QueryAsync<CalendarDayMetric>(sql, new
        {
            calendarDayId = calendarDay.Id
        });
        return metrics.ToList();
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
