using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.HomeContent.Faqs.Commands;

public sealed record CreateFaqCommand(
    string QuestionAr,
    string QuestionEn,
    string AnswerAr,
    string AnswerEn,
    int DisplayOrder,
    bool IsActive) : IRequest<IResult<Guid>>;

