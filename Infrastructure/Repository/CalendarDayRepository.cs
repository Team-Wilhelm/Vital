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

    public async Task<CalendarDay?> GetByDate(DateTimeOffset dateTimeOffset, Guid userId)
    {
        var sql = @"SELECT * FROM ""CalendarDay"" WHERE ""Date""=@dateTimeOffset AND ""UserId""=@userId";
        var calendarDay = await _db.QuerySingleOrDefaultAsync<CalendarDay>(sql, new { dateTimeOffset, userId });
        return calendarDay;
    }
    
    public async Task<CalendarDay?> GetById(Guid calendarDayId)
    {
        var sql = @"SELECT * FROM ""CalendarDay"" WHERE ""Id""=@calendarDayId";
        var calendarDay = await _db.QuerySingleOrDefaultAsync<CalendarDay>(sql, new { calendarDayId });
        return calendarDay;
    }
}
