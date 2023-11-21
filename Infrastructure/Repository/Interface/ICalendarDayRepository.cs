﻿using Models.Days;

namespace Infrastructure.Repository.Interface;

public interface ICalendarDayRepository
{
    Task<CalendarDay?> GetByDate(Guid userId, DateTimeOffset dateTimeOffset);
    Task<CalendarDay?> GetById(Guid calendarDayId);
}
