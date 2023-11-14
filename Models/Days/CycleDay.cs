namespace Models.Days; 

public class CycleDay : CalendarDay {
    public Guid CycleId { get; set; }
    public Cycle Cycle { get; set; }
    public bool IsPeriodDay { get; set; }
}
