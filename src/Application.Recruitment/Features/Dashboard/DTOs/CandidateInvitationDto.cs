using Tawtheef.Application.Common.Models;

namespace Application.Recruitment.Features.Dashboard.DTOs;

public class CandidateInvitationsDto
{
    public Guid InvitationId { get; set; }
    public required string JobTitle { get; set; }
    public required string DepartmentName { get; set; }
    public required string JobCategory { get; set; }
    public required string JobCategoryBackendName { get; set; }
    public required DropdownOptions InvitationStatus { get; set; }
    
    public DateTimeOffset CreatedDate { get; set; }
}
