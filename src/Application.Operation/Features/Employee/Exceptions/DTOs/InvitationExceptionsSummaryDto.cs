namespace Application.Operation.Features.Employee.Exceptions.DTOs;

public sealed record InvitationExceptionsSummaryDto(
    int Total,
    int ReadyToSend,
    int InvitationSent,
    int Applied,
    int Expired,
    int Cancelled);
