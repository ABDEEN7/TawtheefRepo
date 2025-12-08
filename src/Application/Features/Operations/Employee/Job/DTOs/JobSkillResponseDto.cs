using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobSkillResponseDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public required Guid SkillId { get; set; }
    public required DropdownOptions? Skill { get; set; }
    public required bool ShowToApplicants { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}
