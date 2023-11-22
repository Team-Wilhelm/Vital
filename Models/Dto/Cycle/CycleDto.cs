using Models.Days;

namespace Models.Dto.Cycle;

public class CycleDto
{
    public Guid Id { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public Guid UserId { get; set; }
    public List<CycleDay> CycleDays { get; set; }
}
