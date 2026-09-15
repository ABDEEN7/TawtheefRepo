using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.QuestionBankRequests.Commands;

public sealed record CreateQuestionBankRequestCommand(
    Guid QuestionBankTypeId, Guid? ManagementId, Guid? JobTitleId, Guid? StageId, string? Reason)
    : IRequest<IResult<Guid>>;
