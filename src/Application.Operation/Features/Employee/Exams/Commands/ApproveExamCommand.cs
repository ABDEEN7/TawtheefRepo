using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Exams.Commands;

public sealed record ApproveExamCommand(Guid ExamId) : IRequest<IResult<Unit>>;
