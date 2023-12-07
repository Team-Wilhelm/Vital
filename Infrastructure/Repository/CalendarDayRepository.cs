using System.Data;
using Dapper;
using Infrastructure.Data;
using Infrastructure.Repository.Interface;
using Models.Days;

namespace Infrastructure.Repository;

public class CalendarDayRepository : ICalendarDayRepository
{
    private readonly IDbConnection _db;
    private readonly ApplicationDbContext _applicationDbContext;

    public CalendarDayRepository(IDbConnection db, ApplicationDbContext applicationDbContext)
    {
        _db = db;
        _applicationDbContext = applicationDbContext;
    }

    public async Task<CalendarDay?> GetByDate(Guid userId, DateTimeOffset date)
    {
        var sql =
            @"SELECT ""State"" FROM ""CalendarDay"" WHERE ""UserId""=@userId AND CAST(""Date"" AS DATE) = CAST(@date AS DATE)";
        var state = await _db.QuerySingleOrDefaultAsync<string>(sql, new { userId, date });

        sql =
            @"SELECT * FROM ""CalendarDay"" WHERE ""UserId""=@userId AND CAST(""Date"" AS DATE) = CAST(@date AS DATE)";

        return state is null ? null : BuildCalendarDay(state, sql, new { userId, date });
    }

    public async Task<List<CalendarDay>> GetByDateRange(Guid userId, DateTimeOffset from, DateTimeOffset to)
    {
        var sql =
            @"SELECT ""State"" FROM ""CalendarDay"" WHERE ""UserId"" = @userId AND CAST(""Date"" AS DATE) >= CAST(@from AS DATE) AND CAST(""Date"" AS DATE) <= CAST(@to AS DATE)";
        var states = await _db.QueryAsync<string>(sql, new { userId, from, to });

        sql =
            @"SELECT * FROM ""CalendarDay"" WHERE ""UserId"" = @userId AND CAST(""Date"" AS DATE) >= CAST(@from AS DATE) AND CAST(""Date"" AS DATE) <= CAST(@to AS DATE)";

        var parameters = new { userId, from, to };
        var calendarDays = new List<CalendarDay>();

        foreach (var state in states)
        {
            var calendarDay = BuildCalendarDay(state!, sql, parameters);
            if (calendarDay is not null)
            {
                calendarDays.Add(calendarDay);
            }
        }

        return calendarDays;
    }

    public async Task<CalendarDay?> GetById(Guid calendarDayId)
    {
        var sql = @"SELECT ""State"" FROM ""CalendarDay"" WHERE ""Id""=@calendarDayId";
        var state = await _db.QuerySingleOrDefaultAsync<string>(sql, new { calendarDayId });

        sql = @"SELECT * FROM ""CalendarDay"" WHERE ""Id""=@calendarDayId";
        return state is null ? null : BuildCalendarDay(state!, sql, new { calendarDayId });
    }

    public async Task<CalendarDay> CreteCycleDay(Guid userId, DateTimeOffset dateTime, Guid? cycleId)
    {
        var lastCycle = _applicationDbContext.Cycles.Where(c => c.UserId == userId)
            .OrderByDescending(c => c.StartDate)
            .FirstOrDefault();
        if (lastCycle is null && cycleId is null)
        {
            throw new Exception("User has no cycle yet");
        }

        cycleId ??= lastCycle!.Id;

        // Set time to 12:00:00
        dateTime = new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, 12, 0, 0, dateTime.Offset);
        var cycleDay = new CycleDay()
        {
            CycleId = cycleId.Value,
            UserId = userId,
            Date = dateTime,
        };

        // TODO: please rework me into Dapper
        await _applicationDbContext.CycleDays.AddAsync(cycleDay);
        await _applicationDbContext.SaveChangesAsync();
        return cycleDay;
    }

    public CalendarDay? BuildCalendarDay(string state, string sql, object param)
    {
        return state switch
        {
            "CycleDay" => _db.QuerySingleOrDefault<CycleDay>(sql, param),
            _ => throw new InvalidOperationException(
                $"There was an issue while creating a calendar day. Invalid state, {state}")
        };
    }

    public async Task<IEnumerable<CycleDay>> GetCycleDaysForSpecifiedPeriodAsync(Guid userId, DateTimeOffset startDate,
        DateTimeOffset endDate)
    {
        var sql =
            @"SELECT * FROM ""CalendarDay"" WHERE ""Date"" BETWEEN @startDate AND @endDate AND ""UserId""=@userId";
        var calendarDays = await _db.QueryAsync<CycleDay>(sql, new { startDate, endDate, userId });
        return calendarDays;
    }

    public async Task SetIsPeriod(Guid cycleDayId, bool isPeriod)
    {
        var sql = @"UPDATE ""CalendarDay"" SET ""IsPeriod"" = @isPeriod WHERE ""Id"" = @calendarDayId";
        await _db.ExecuteAsync(sql, new { calendarDayId = cycleDayId, isPeriod });
    }

    public async Task Delete(Guid calendarDayId)
    {
        var sql = @"DELETE FROM ""CalendarDay"" WHERE ""Id"" = @calendarDayId";
        await _db.ExecuteAsync(sql, new { calendarDayId });
    }

    public async Task UpdateCycleIds(Guid oldCycleId, Guid newCycleId, DateTimeOffset from, DateTimeOffset to)
    {
        var sql = @"UPDATE ""CalendarDay"" SET ""CycleId"" = @newCycleId WHERE ""CycleId"" = @oldCycleId 
                    AND ""Date"" BETWEEN @from AND @to";
        await _db.ExecuteAsync(sql, new { oldCycleId, newCycleId, from, to });
    }
}
