using Tawtheef.Application.Common.Models;

namespace Application.Operation.Features.Employee.JobInvitationSummaryDetails.DTOs;

public sealed class JobInvitationSummaryDetailsRowDto
{
    public Guid InviteId { get; init; }
    public string FullName { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string PersonalNumber { get; set; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public DropdownOptions Status { get; set; } = new();
    public Guid  BatchNumber { get; init; }
    public DateTimeOffset SentDate { get; init; }
    public DateTimeOffset? ReadDate { get; set; }
    public DateTimeOffset? AppliedDate { get; init; }
    public DateTimeOffset? DeclinedDate { get; set; }
    public DateTimeOffset? ExpiredDate { get; set; }
}
