namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;

public class PointDetailDto
{
    public string Section { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public string? SectionAr { get; set; }
    public string? SectionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    
    public List<string> ItemsAr { get; set; } = new();
    public List<string> ItemsEn { get; set; } = new();
    public int Points { get; set; }
}
