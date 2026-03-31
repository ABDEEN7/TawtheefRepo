using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.Commands;

public record ReviewInvitationAttachmentCommand(
    Guid InvitationId,
    Guid AttachmentId,
    bool IsApproved,
    string? ReviewNote
) : IRequest<Result<Unit>>;
