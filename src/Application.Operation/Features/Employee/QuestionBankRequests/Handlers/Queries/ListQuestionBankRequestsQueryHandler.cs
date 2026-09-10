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
            .WhereIf(request.JobTitleId.HasValue, x => x.QuestionBank.JobTitleId == request.JobTitleId)
            .OrderByDescending(x => x.SubmittedAt).ThenBy(x => x.Id);

        return Result.Ok(await query.ToPaginatedListAsync<QuestionBankRequest, QuestionBankRequestListItemDto>(mapper, request, cancellationToken));
    }
}
