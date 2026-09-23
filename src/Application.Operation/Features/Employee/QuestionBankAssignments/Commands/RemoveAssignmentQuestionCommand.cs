using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.QuestionBankAssignments.Commands;

public sealed record RemoveAssignmentQuestionCommand(Guid AssignmentId, Guid ItemId)
    : IRequest<IResult<Unit>>;
