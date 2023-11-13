namespace Models.Days; 

public abstract class CalendarDay {
    public DateTimeOffset Date { get; set; }
    public List<Metrics> Metrics { get; set; }
    public string State { get; set; } = "Period"; // Period, Pregnancy, Menopause
}
