using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.CandidateExamAttempts.Commands;

public sealed record RescheduleInterruptedCandidateExamCommand(
    Guid TestAttemptId,
    Guid OfficerId,
    Guid TargetTestSessionId,
    string? Reason) : IRequest<IResult<Guid>>;
