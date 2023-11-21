namespace Models.Days;

public class PredictedPeriodDay : CalendarDay
{
    public Guid CycleId { get; set; }
    public Cycle Cycle { get; set; }
}
