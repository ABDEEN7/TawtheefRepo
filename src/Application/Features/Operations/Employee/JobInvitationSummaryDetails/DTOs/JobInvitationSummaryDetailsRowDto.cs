using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.DTOs;

public sealed class JobInvitationSummaryDetailsRowDto
{
    public Guid InviteId { get; init; }
    public string FullName { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string PersonalNumber { get; set; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public DropdownOptions Status { get; set; } = new();
    public int BatchNumber { get; init; }
    public DateTimeOffset SentDate { get; init; }
    public DateTimeOffset? ReadDate { get; init; }
    public DateTimeOffset? AppliedDate { get; init; }
    public DateTimeOffset? DeclinedDate { get; init; }
    public DateTimeOffset? ExpiredDate { get; init; }
}
