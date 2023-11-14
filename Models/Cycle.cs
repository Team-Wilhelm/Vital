using System.ComponentModel.DataAnnotations.Schema;
using Models.Days;
using Models.Identity;

namespace Models; 

public class Cycle {
    public Guid Id { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; }
    public List<CycleDay> CycleDays { get; set; }
    [NotMapped]
    public Period Period { get; set; }
}
