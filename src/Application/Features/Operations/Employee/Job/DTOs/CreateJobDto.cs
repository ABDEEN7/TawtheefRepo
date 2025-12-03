using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Dtos;

public class CreateJobDto
{
    // Basic Information
    public required string Title { get; set; }
    public required int Vacancies { get; set; }
    public DateTimeOffset Deadline { get; set; }
    public required string Description { get; set; }
    public required string Benefits { get; set; }
    public string? Overview { get; set; }
    public string? QualificationsDescription { get; set; }
    public DateTimeOffset? PublishAt { get; set; }

    // Requirements
    public int MinimumExperienceYears { get; set; }
    public int MinimumAge { get; set; }
    public int MaximumAge { get; set; }

    // Foreign keys
    public Guid SectorId { get; set; }
    public Guid ManagementId { get; set; }
    public Guid RequestingDepartmentId { get; set; }
    public Guid JobCategoryId { get; set; }
    public Guid GenderId { get; set; }
    public Guid WorkLocationId { get; set; }
    public Guid MajorId { get; set; }
    public Guid? SubMajorId { get; set; }
    public Guid WorkTypeId { get; set; }
    public Guid? StatusId { get; set; }

    // Quota
    public JobQuotaDto Quota { get; set; } = default!;

    // Collections
    public List<JobDegreeDto> Degrees { get; set; } = [];
    public List<JobConditionDto> Conditions { get; set; } = [];
    public List<JobSkillDto> Skills { get; set; } = [];
    public List<JobResponsibilityDto> Responsibilities { get; set; } = [];
    public List<RequiredAttachmentDto> RequiredAttachments { get; set; } = [];
}
