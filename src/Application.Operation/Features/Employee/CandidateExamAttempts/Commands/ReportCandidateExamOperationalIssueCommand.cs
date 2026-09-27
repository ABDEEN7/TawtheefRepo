using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.CandidateExamAttempts.Commands;

public sealed record ReportCandidateExamOperationalIssueCommand(
    Guid TestAttemptId,
    Guid OfficerId,
    string Reason)
    : IRequest<IResult<Unit>>;
