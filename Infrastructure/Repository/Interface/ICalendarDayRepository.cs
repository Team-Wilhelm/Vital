using Models.Days;

namespace Infrastructure.Repository.Interface;

public interface ICalendarDayRepository
{
    Task<CalendarDay?> GetByDate(DateTimeOffset dateTimeOffset, Guid userId);
    Task<CalendarDay?> GetById(Guid calendarDayId);
}
