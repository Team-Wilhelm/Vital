using Models.Days;

namespace Infrastructure.Repository.Interface;

public interface ICalendarDayRepository
{
    Task<CalendarDay?> GetByDate(Guid userId, DateTimeOffset dateTimeOffset);
    Task<List<CalendarDay>> GetByDateRange(Guid userId, DateTimeOffset from, DateTimeOffset to);
    /// <summary>
    /// Creates a CalendarDay with delimiter CycleDay and ties it to a Cycle if specified, otherwise it will select the current cycle.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="dateTime"></param>
    /// <param name="cycleId"></param>
    /// <returns></returns>
    Task<CalendarDay> CreteCycleDay(Guid userId, DateTimeOffset dateTime, Guid? cycleId);
    Task<CalendarDay?> GetById(Guid calendarDayId);
    CalendarDay? BuildCalendarDay(string state, string sql, object parameters);
    Task<IEnumerable<CycleDay>> GetCycleDaysForSpecifiedPeriodAsync(Guid userId, DateTimeOffset startDate, DateTimeOffset endDate);
    Task SetIsPeriod(Guid cycleDayId, bool isPeriod);
    Task Delete(Guid calendarDayId);
    Task UpdateCycleIds(Guid oldCycleId, Guid newCycleId, DateTimeOffset from, DateTimeOffset to);
}
