using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.CandidateExamAttempts.Commands;

public sealed record AuthorizeCandidateExamResumeCommand(
    Guid TestAttemptId,
    Guid OfficerId,
    string? ResolutionNotes) : IRequest<IResult<Unit>>;
