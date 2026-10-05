using Application.Operation.Features.Employee.QuestionBankRequests.DTOs;
using Application.Operation.Features.Employee.QuestionBankRequests.Queries;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Application.Operation.Features.Employee.QuestionBankRequests.Handlers.Queries;

public sealed class ListQuestionBankRequestsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<ListQuestionBankRequestsQuery, IResult<PaginatedResult<QuestionBankRequestListItemDto>>>
{
    public async Task<IResult<PaginatedResult<QuestionBankRequestListItemDto>>> Handle(
        ListQuestionBankRequestsQuery request, CancellationToken cancellationToken)
    {
        var search = request.Search?.Trim();
        var query = unitOfWork.GetEntityRepository<QuestionBankRequest>().DbSet.AsNoTracking()
            .Include(x => x.RequestType)
            .Include(x => x.Status)
            .Include(x => x.SubmittedBy)
            .Include(x => x.QuestionBank)
            .Include(x => x.QuestionBank.QuestionBankType)
            .Include(x => x.QuestionBank.Management)
            .Include(x => x.QuestionBank.JobTitle)
            .WhereIf(!string.IsNullOrWhiteSpace(search), x =>
                EF.Functions.Like(x.RequestType.NameAr, $"%{search}%") || EF.Functions.Like(x.RequestType.NameEn, $"%{search}%") ||
                EF.Functions.Like(x.Status.NameAr, $"%{search}%") || EF.Functions.Like(x.Status.NameEn, $"%{search}%") ||
                EF.Functions.Like(x.QuestionBank.QuestionBankType.NameAr, $"%{search}%") || EF.Functions.Like(x.QuestionBank.QuestionBankType.NameEn, $"%{search}%") ||
                (x.QuestionBank.Management != null && (EF.Functions.Like(x.QuestionBank.Management.NameAr, $"%{search}%") || EF.Functions.Like(x.QuestionBank.Management.NameEn, $"%{search}%"))) ||
                (x.QuestionBank.JobTitle != null && (EF.Functions.Like(x.QuestionBank.JobTitle.JobNameAr, $"%{search}%") || EF.Functions.Like(x.QuestionBank.JobTitle.JobNameEn, $"%{search}%"))) ||
                (x.SubmittedBy.FullNameAr != null && EF.Functions.Like(x.SubmittedBy.FullNameAr, $"%{search}%")) ||
                (x.SubmittedBy.FullNameEn != null && EF.Functions.Like(x.SubmittedBy.FullNameEn, $"%{search}%")))
            .WhereIf(request.RequestTypeId.HasValue, x => x.RequestTypeId == request.RequestTypeId)
            .WhereIf(request.QuestionBankTypeId.HasValue, x => x.QuestionBank.QuestionBankTypeId == request.QuestionBankTypeId)
            .WhereIf(request.StatusId.HasValue, x => x.StatusId == request.StatusId)
            .WhereIf(request.ManagementId.HasValue, x => x.QuestionBank.ManagementId == request.ManagementId)
            .WhereIf(request.JobTitleId.HasValue, x => x.QuestionBank.JobTitleId == request.JobTitleId);

        var sorted = ApplySorting(query, request.SortBy, request.SortDirection);

        return Result.Ok(await sorted.ToPaginatedListAsync<QuestionBankRequest, QuestionBankRequestListItemDto>(
            mapper, request with { SortBy = null, SortDirection = null }, cancellationToken));
    }

    private static IOrderedQueryable<QuestionBankRequest> ApplySorting(
        IQueryable<QuestionBankRequest> query,
        string? sortBy,
        string? sortDirection)
    {
        var descending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        return sortBy?.Trim().ToLowerInvariant() switch
        {
            "requesttype" => Order(query, x => x.RequestType.NameEn, descending),
            "questionbanktype" => Order(query, x => x.QuestionBank.QuestionBankType.NameEn, descending),
            "management" => Order(query, x => x.QuestionBank.Management == null ? string.Empty : x.QuestionBank.Management.NameEn, descending),
            "jobtitle" => Order(query, x => x.QuestionBank.JobTitle == null ? string.Empty : x.QuestionBank.JobTitle.JobNameEn, descending),
            "status" => Order(query, x => x.Status.DisplayOrder, descending),
            "submittedby" => Order(query, x => x.SubmittedBy.FullNameEn ?? string.Empty, descending),
            "submittedat" => Order(query, x => x.SubmittedAt, descending),
            _ => query.OrderByDescending(x => x.SubmittedAt).ThenBy(x => x.Id)
        };
    }

    private static IOrderedQueryable<QuestionBankRequest> Order<TKey>(
        IQueryable<QuestionBankRequest> query,
        System.Linq.Expressions.Expression<Func<QuestionBankRequest, TKey>> key,
        bool descending) => descending
            ? query.OrderByDescending(key).ThenBy(x => x.Id)
            : query.OrderBy(key).ThenBy(x => x.Id);
}
