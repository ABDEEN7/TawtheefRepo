using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Exams.Commands;

public sealed record AcceptExamResultCandidateCalculatedScoreCommand(
    Guid ReportId,
    Guid CandidateResultId,
    string? ResolutionNotes) : IRequest<IResult<Unit>>;
