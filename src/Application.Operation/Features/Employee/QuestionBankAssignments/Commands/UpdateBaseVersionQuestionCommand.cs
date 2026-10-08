using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.QuestionBankAssignments.Commands;

public sealed record UpdateBaseVersionQuestionCommand(Guid AssignmentId, Guid QuestionId, QuestionInput Question)
    : IRequest<IResult<Guid>>;
