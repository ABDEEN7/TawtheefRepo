using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.QuestionBankAssignments.Commands;

public sealed record EditAssignmentQuestionCommand(Guid AssignmentId, Guid ItemId, QuestionInput Question)
    : IRequest<IResult<Unit>>;
