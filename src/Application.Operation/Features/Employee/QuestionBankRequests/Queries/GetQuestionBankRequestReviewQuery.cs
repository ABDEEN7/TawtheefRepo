using Application.Operation.Features.Employee.QuestionBankRequests.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.QuestionBankRequests.Queries;

public sealed record GetQuestionBankRequestReviewQuery(Guid RequestId)
    : IRequest<IResult<QuestionBankRequestReviewDto>>;
