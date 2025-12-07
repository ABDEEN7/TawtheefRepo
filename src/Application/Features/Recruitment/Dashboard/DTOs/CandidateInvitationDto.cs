using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Recruitment.Dashboard.DTOs;

public class CandidateInvitationsDto
{
    public Guid InvitationId { get; set; }
    public required string JobTitle { get; set; }
    public required string DepartmentName { get; set; }
    public required string JobCategory { get; set; }
    public required DropdownOptions InvitationStatus { get; set; }
    public DateTime CreateDate { get; set; }
}
