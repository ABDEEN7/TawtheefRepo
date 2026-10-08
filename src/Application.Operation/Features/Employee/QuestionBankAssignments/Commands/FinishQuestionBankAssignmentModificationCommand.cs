using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.QuestionBankAssignments.Commands;

public sealed record FinishQuestionBankAssignmentModificationCommand(Guid AssignmentId)
    : IRequest<IResult<Unit>>;
