using Application.Operation.Features.Employee.QuestionBankRequests.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.QuestionBankRequests.Queries;

public sealed record GetQuestionBankRequestDetailsQuery(Guid Id) : IRequest<IResult<QuestionBankRequestDetailsDto>>;
public sealed record GetEligibleQuestionBankEmployeesQuery(Guid RequestId, string? Search) : IRequest<IResult<IReadOnlyCollection<QuestionBankEmployeeLookupDto>>>;
