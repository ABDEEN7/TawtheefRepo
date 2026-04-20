namespace Application.Operation.Features.Employee.JobManagement.Job.DTOs;

public class UpdateJobQualificationsDto
{
    public Guid? MajorId { get; set; }
    public Guid? SubMajorId { get; set; }
    public string? QualificationsDescriptionAr { get; set; }
    public string? QualificationsDescriptionEn { get; set; }
    public List<JobDegreeRequestDto>? Degrees { get; set; }
    public List<JobSpecializationRequestDto>? JobSpecializations { get; set; }
}
