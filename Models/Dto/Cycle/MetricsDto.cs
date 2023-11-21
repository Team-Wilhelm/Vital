namespace Models.Dto.Cycle;

public class MetricsDto
{
    public Guid Id { get; set; }
    public Guid CalendarDayId { get; set; }
    /// <example>Headache</example>
    public required string Name { get; set; }
    /// <example>Ache in the head</example>
    public string? Description { get; set; }
    /// <example>["None", "Mild", "Moderate", "Severe"] or ["Yes", "No"]</example>
    public List<string> Values { get; set; } = new();
    /// <example>"Mild" or "Yes"</example>
    public string? SelectedValue { get; set; }
}
