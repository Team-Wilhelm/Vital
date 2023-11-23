using Models.Days;

namespace Infrastructure.Repository.Interface;

public interface ICalendarDayRepository
{
    Task<CalendarDay?> GetByDate(Guid userId, DateTimeOffset dateTimeOffset);
    Task<List<CalendarDay>> GetByDateRange(Guid userId, DateTimeOffset from, DateTimeOffset to);
    Task<CalendarDay?> GetById(Guid calendarDayId);
}
