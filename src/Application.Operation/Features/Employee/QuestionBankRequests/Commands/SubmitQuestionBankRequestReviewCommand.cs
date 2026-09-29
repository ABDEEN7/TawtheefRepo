using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.QuestionBankRequests.Commands;

public sealed record SubmitQuestionBankRequestReviewCommand(
    Guid RequestId, IReadOnlyCollection<QuestionReviewInput> Reviews)
    : IRequest<IResult<SubmitQuestionBankRequestReviewResult>>;

public sealed record QuestionReviewInput(
    Guid RequestItemId, Guid ReviewedRevisionId, Guid DecisionId, string? ReviewNote);

public sealed record SubmitQuestionBankRequestReviewResult(bool ChangesRequired, int ReviewRound);
