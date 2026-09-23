using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.QuestionBankAssignments.Commands;

public sealed record AddQuestionToAssignmentCommand(Guid AssignmentId, QuestionInput Question)
    : IRequest<IResult<Guid>>;
