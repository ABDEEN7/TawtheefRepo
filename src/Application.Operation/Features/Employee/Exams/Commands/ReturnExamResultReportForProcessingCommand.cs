using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Exams.Commands;

public sealed record ReturnExamResultReportForProcessingCommand(Guid ReportId, string? Reason)
    : IRequest<IResult<Unit>>;
