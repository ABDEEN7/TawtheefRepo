using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Exams.Commands;

public sealed record RescheduleExamResultCandidateCommand(
    Guid ReportId,
    Guid CandidateResultId,
    Guid TargetTestSessionId,
    string? Reason) : IRequest<IResult<Guid>>;
