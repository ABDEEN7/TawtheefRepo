using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Exams.Commands;

public sealed record ApproveExamResultReportCommand(
    Guid ReportId,
    string? DecisionNotes)
    : IRequest<IResult<Unit>>;
