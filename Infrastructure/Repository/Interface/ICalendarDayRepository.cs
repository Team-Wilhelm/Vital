using Models.Days;

namespace Infrastructure.Repository.Interface;

public interface ICalendarDayRepository
{
    /// <summary>
    /// Retrieves a `CalendarDay` by user and specific date.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="date">The date.</param>
    /// <returns>The `CalendarDay` for the user at the specified date or null if no record found.</returns>
    Task<CalendarDay?> GetByDate(Guid userId, DateTimeOffset date);

    /// <summary>
    /// Retrieves a list of `CalendarDay` within a date range for a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="from">The start date.</param>
    /// <param name="to">The end date.</param>
    /// <returns>A list of `CalendarDay` within the specified date range.</returns>
    Task<List<CalendarDay>> GetByDateRange(Guid userId, DateTimeOffset from, DateTimeOffset to);

    /// <summary>
    /// Retrieves a `CalendarDay` by its ID.
    /// </summary>
    /// <param name="calendarDayId">The `CalendarDay` ID.</param>
    /// <returns>The `CalendarDay` with the specified ID or null if no record is found.</returns>
    Task<CalendarDay?> GetById(Guid calendarDayId);

    /// <summary>
    /// Creates a 'CycleDay' of `CalendarDay` and attaches it to a Cycle.
    /// </summary>
    /// <param name="userId">The User ID.</param>
    /// <param name="dateTime">The DateTime value.</param>
    /// <param name="cycleId">Optional cycle ID to tie 'CycleDay' to</param>
    /// <returns>The created 'CycleDay'.</returns>
    Task<CalendarDay> CreteCycleDay(Guid userId, DateTimeOffset dateTime, Guid? cycleId);

    /// <summary>
    /// Constructs a `CalendarDay` object from given state and parameters.
    /// </summary>
    /// <param name="state">The state of the `CalendarDay` object.</param>
    /// <param name="sql">The SQL query string.</param>
    /// <param name="param">The parameters for the SQL query.</param>
    /// <returns>The contructed `CalendarDay` object.</returns>
    public CalendarDay? BuildCalendarDay(string state, string sql, object param);

    /// <summary>
    /// Retrieves a list of `CycleDay` within a specified period for a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <returns>A list of `CycleDay` within the specified period.</returns>
    Task<IEnumerable<CycleDay>> GetCycleDaysForSpecifiedPeriodAsync(Guid userId, DateTimeOffset startDate,
        DateTimeOffset endDate);

    /// <summary>
    /// Sets the 'IsPeriod' property of a specific `CalendarDay` record.
    /// </summary>
    /// <param name="cycleDayId">The `CalendarDay` ID.</param>
    /// <param name="isPeriod">The boolean value to be set.</param>
    Task SetIsPeriod(Guid cycleDayId, bool isPeriod);

    /// <summary>
    /// Deletes a specific `CalendarDay` record.
    /// </summary>
    /// <param name="calendarDayId">The `CalendarDay` ID.</param>
    Task Delete(Guid calendarDayId);

    /// <summary>
    /// Updates `CalendarDay` records by replacing old Cycle IDs with new ones within a specified date range.
    /// </summary>
    /// <param name="oldCycleId">The old Cycle ID.</param>
    /// <param name="newCycleId">The new Cycle ID.</param>
    /// <param name="from">The start date.</param>
    /// <param name="to">The end date.</param>
    Task UpdateCycleIds(Guid oldCycleId, Guid newCycleId, DateTimeOffset from, DateTimeOffset to);
}
