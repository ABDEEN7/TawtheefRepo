using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.QuestionBankRequests.Commands;

public sealed record CreateQuestionBankMaintenanceRequestCommand(Guid QuestionBankId, string? Reason)
    : IRequest<IResult<Guid>>;
