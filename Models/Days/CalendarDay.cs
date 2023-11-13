namespace Models.Days; 

public abstract class CalendarDay {
    public DateTimeOffset Date { get; set; }
    public List<Symptom> Symptoms { get; set; }
}
