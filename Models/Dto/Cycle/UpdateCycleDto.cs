using System.ComponentModel.DataAnnotations;

namespace Models.Dto.Cycle;

public class UpdateCycleDto
{
    [Required]
    public DateTimeOffset StartDate { get; set; }
    [Required]
    public DateTimeOffset EndDate { get; set; }
}
