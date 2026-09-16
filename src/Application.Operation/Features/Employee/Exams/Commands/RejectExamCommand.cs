using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Exams.Commands;

public sealed record RejectExamCommand(Guid ExamId, string? Note) : IRequest<IResult<Unit>>;
