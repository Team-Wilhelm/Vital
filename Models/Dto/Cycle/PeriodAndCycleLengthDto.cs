using System.ComponentModel.DataAnnotations;

namespace Models.Dto.Cycle;

public class PeriodAndCycleLengthDto
{
    [MinLength(1)]
    public float CycleLength { get; set; } = 28;

    [MinLength(1)]
    public float PeriodLength { get; set; } = 5;
}
