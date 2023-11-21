using System.Data;
using Dapper;
using Infrastructure.Repository.Interface;
using Models.Days;

namespace Infrastructure.Repository;

public class CalendarDayRepository : ICalendarDayRepository
{
    private readonly IDbConnection _db;

    public CalendarDayRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<CalendarDay?> GetByDate(Guid userId, DateTimeOffset date)
    {
        var sql =  @"SELECT ""State"" FROM ""CalendarDay"" WHERE ""UserId""=@userId AND CAST(""Date"" AS DATE) = CAST(@date AS DATE)";
        var state = await _db.QuerySingleOrDefaultAsync<string>(sql, new { userId, date });
        
        sql = @"SELECT * FROM ""CalendarDay"" WHERE ""UserId""=@userId AND CAST(""Date"" AS DATE) = CAST(@date AS DATE)";
        
        return state switch
        {
            "CycleDay" => await _db.QuerySingleOrDefaultAsync<CycleDay>(sql, new { userId, date }),
            _ => throw new InvalidOperationException("Invalid state")
        };
    }
    
    public async Task<CalendarDay?> GetById(Guid calendarDayId)
    {
        var sql = @"SELECT * FROM ""CalendarDay"" WHERE ""Id""=@calendarDayId";
        var calendarDay = await _db.QuerySingleOrDefaultAsync<CalendarDay>(sql, new { calendarDayId });
        return calendarDay;
    }
}
