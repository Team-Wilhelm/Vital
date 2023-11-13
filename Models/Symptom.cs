using System.ComponentModel.DataAnnotations;

namespace Models; 

public class Symptom {
    public required string Name { get; set; } // For example: "Headache"
    public string? Description { get; set; } // For example: "Ache in the head"
    public List<string> Values { get; set; } = new(); // For example: ["None", "Mild", "Moderate", "Severe"] or ["Yes", "No"]
    public string? SelectedValue { get; set; } // For example: "Mild" or "Yes"
}
