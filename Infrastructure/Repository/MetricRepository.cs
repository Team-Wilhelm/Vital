using System.Data;
using System.Runtime.InteropServices.JavaScript;
using Dapper;
using Infrastructure.Repository.Interface;
using Models;
using Models.Pagination;

namespace Infrastructure.Repository;

public class MetricRepository : IMetricRepository
{
    private readonly IDbConnection _db;

    public MetricRepository(IDbConnection db)
    {
        _db = db;
    }
    
    public async Task<PaginatedList<Metrics>> Get(Guid userId, DateTimeOffset date, Paginator paginator)
    {
        var calendarDayId = await GetCalendarDayId(userId, date);
        
        var sql = @"SELECT * FROM ""Metrics"" INNER JOIN ""CalendarDayMetrics"" ON ""MetricsId""=""Id"" 
                    WHERE ""CalendarDaysId""=@calendarDayId LIMIT @limit OFFSET @offset";
        
        var count = _db.QuerySingle<int>(@"SELECT COUNT(*) FROM ""Metrics"" INNER JOIN ""CalendarDayMetrics"" ON ""MetricsId""=""Id"" 
                    WHERE ""CalendarDaysId""=@calendarDayId", new {calendarDayId});
        
        var metricList = await _db.QueryAsync<Metrics>(sql, new
        {
            calendarDayId, 
            limit=paginator.ItemsPerPage, 
            offset=(paginator.Page - 1) * paginator.ItemsPerPage
        });
        return await PaginatedList<Metrics>.CreateAsync(metricList, paginator.Page, paginator.ItemsPerPage, count);
    }

    public async Task<Guid> GetCalendarDayId(Guid userId, DateTimeOffset date)
    {
        var sql = @"SELECT ""Id"" FROM ""CalendarDay"" WHERE ""UserId""=@userId AND ""Date""=@date";
        var calendarDayId = await _db.QuerySingleOrDefaultAsync<Guid>(sql, new { userId, date });
        return calendarDayId;
    }
}
