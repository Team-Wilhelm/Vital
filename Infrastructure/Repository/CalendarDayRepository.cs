﻿using System.Data;
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
        var sql = @"SELECT ""State"" FROM ""CalendarDay"" WHERE ""UserId""=@userId AND CAST(""Date"" AS DATE) = CAST(@date AS DATE)";
        var state = await _db.QuerySingleOrDefaultAsync<string>(sql, new { userId, date });

        sql = @"SELECT * FROM ""CalendarDay"" WHERE ""UserId""=@userId AND CAST(""Date"" AS DATE) = CAST(@date AS DATE)";

        return CreateCalendarDay(state!, sql, new { userId, date });
    }

    public async Task<CalendarDay?> GetById(Guid calendarDayId)
    {
        var sql = @"SELECT ""State"" FROM ""CalendarDay"" WHERE ""Id""=@calendarDayId";
        var state = await _db.QuerySingleOrDefaultAsync<string>(sql, new { calendarDayId });

        sql = @"SELECT * FROM ""CalendarDay"" WHERE ""Id""=@calendarDayId";
        return CreateCalendarDay(state!, sql, new { calendarDayId });
    }

    private CalendarDay? CreateCalendarDay(string state, string sql, object param)
    {
        return state switch
        {
            "CycleDay" => _db.QuerySingleOrDefault<CycleDay>(sql, param),
            _ => throw new InvalidOperationException("Invalid state")
        };
    }
}
