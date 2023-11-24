using Models.Days;

namespace Infrastructure.Repository.Interface;

public interface ICalendarDayRepository
{
    Task<CalendarDay?> GetByDate(Guid userId, DateTimeOffset dateTimeOffset);
    Task<List<CalendarDay>> GetByDateRange(Guid userId, DateTimeOffset from, DateTimeOffset to);
    Task<CalendarDay> CreteCycleDay(Guid UserId, DateTimeOffset dateTime);
    Task<CalendarDay?> GetById(Guid calendarDayId);
    CalendarDay? BuildCalendarDay(string state, string sql, object parameters);
    Task<IEnumerable<CycleDay>> GetCycleDaysForSpecifiedPeriodAsync(Guid userId, DateTimeOffset startDate, DateTimeOffset endDate);
}
