using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.HomeContent.Faqs.Commands;

public sealed record UpdateFaqCommand(
    Guid FaqId,
    string QuestionAr,
    string QuestionEn,
    string AnswerAr,
    string AnswerEn,
    int DisplayOrder,
    bool IsActive) : IRequest<IResult<Unit>>;

