using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.DTOs;

public sealed class JobInvitationSummaryDetailsRowDto
{
    public Guid InviteId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Nationality { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public DropdownOptions Status { get; init; } = new();
    public DateTimeOffset SentDate { get; init; }
}
