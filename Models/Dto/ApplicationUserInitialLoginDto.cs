using System.ComponentModel.DataAnnotations;

namespace Models.Dto;

public class ApplicationUserInitialLoginDto
{
    public Guid Id { get; set; }
    
    [Range(0.0, 100, ErrorMessage = "The field {0} must be greater than {1}.")]
    public float? CycleLength { get; set; }
    
    [Range(0.0, 100, ErrorMessage = "The field {0} must be greater than {1}.")]
    public float? PeriodLength { get; set; }
}
