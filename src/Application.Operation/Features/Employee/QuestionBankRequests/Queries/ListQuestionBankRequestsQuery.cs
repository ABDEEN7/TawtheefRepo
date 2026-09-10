using Application.Operation.Features.Employee.QuestionBankRequests.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.QuestionBankRequests.Queries;

public sealed record ListQuestionBankRequestsQuery : PaginatedRequest,
    IRequest<IResult<PaginatedResult<QuestionBankRequestListItemDto>>>
{
    public string? Search { get; init; }
    public Guid? RequestTypeId { get; init; }
    public Guid? QuestionBankTypeId { get; init; }
    public Guid? StatusId { get; init; }
    public Guid? ManagementId { get; init; }
    public Guid? JobTitleId { get; init; }
}
