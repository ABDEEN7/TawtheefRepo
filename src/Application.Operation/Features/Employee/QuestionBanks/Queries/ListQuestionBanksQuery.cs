using Application.Operation.Features.Employee.QuestionBanks.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.QuestionBanks.Queries;

public sealed record ListQuestionBanksQuery
    : PaginatedRequest, IRequest<IResult<PaginatedResult<QuestionBankListItemDto>>>
{
    public string? Search { get; init; }
    public Guid? QuestionBankTypeId { get; init; }
    public Guid? ManagementId { get; init; }
    public Guid? JobTitleId { get; init; }
    public Guid? StageId { get; init; }
    public bool? IsActive { get; init; }
}
