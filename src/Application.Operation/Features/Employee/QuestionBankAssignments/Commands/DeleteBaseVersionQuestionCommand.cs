using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.QuestionBankAssignments.Commands;

public sealed record DeleteBaseVersionQuestionCommand(Guid AssignmentId, Guid QuestionId)
    : IRequest<IResult<Guid>>;
