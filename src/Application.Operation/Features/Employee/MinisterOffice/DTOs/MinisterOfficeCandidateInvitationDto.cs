using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.MinisterOffice.DTOs;

public sealed class MinisterOfficeCandidateInvitationDto
{
    public Guid InvitationId { get; init; }
    public InvitationSource Source { get; init; }
    public string JobTitleEn { get; init; } = default!;
    public string JobTitleAr { get; init; } = default!;
    public string OrganizationNameEn { get; init; } = default!;
    public string OrganizationNameAr { get; init; } = default!;
    public DropdownOptions InvitationStatus { get; init; } = default!;
    public DateTime? InvitedAt { get; init; }
}
