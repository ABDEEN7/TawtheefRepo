using Application.Operation.Features.Employee.QuestionBanks.DTOs;
using Application.Operation.Features.Employee.QuestionBanks.Queries;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Application.Operation.Features.Employee.QuestionBanks.Handlers.Queries;

public sealed class ListQuestionBanksQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<ListQuestionBanksQuery, IResult<PaginatedResult<QuestionBankListItemDto>>>
{
    public async Task<IResult<PaginatedResult<QuestionBankListItemDto>>> Handle(
        ListQuestionBanksQuery request,
        CancellationToken cancellationToken)
    {
        var searchTerm = request.Search?.Trim();
        var query = unitOfWork
            .GetEntityRepository<QuestionBank>()
            .DbSet
            .AsNoTracking()
            .Include(questionBank => questionBank.QuestionBankType)
            .Include(questionBank => questionBank.Management)
            .Include(questionBank => questionBank.JobTitle)
            .Include(questionBank => questionBank.Stage)
            .Include(questionBank => questionBank.CurrentApprovedVersion)
            .WhereIf(
                !string.IsNullOrWhiteSpace(searchTerm),
                questionBank =>
                    EF.Functions.Like(questionBank.QuestionBankType.NameAr, $"%{searchTerm}%") ||
                    EF.Functions.Like(questionBank.QuestionBankType.NameEn, $"%{searchTerm}%") ||
                    (questionBank.Management != null &&
                     (EF.Functions.Like(questionBank.Management.NameAr, $"%{searchTerm}%") ||
                      EF.Functions.Like(questionBank.Management.NameEn, $"%{searchTerm}%"))) ||
                    (questionBank.JobTitle != null &&
                     (EF.Functions.Like(questionBank.JobTitle.JobNameAr, $"%{searchTerm}%") ||
                      EF.Functions.Like(questionBank.JobTitle.JobNameEn, $"%{searchTerm}%"))) ||
                    (questionBank.Stage != null &&
                     (EF.Functions.Like(questionBank.Stage.NameAr, $"%{searchTerm}%") ||
                      EF.Functions.Like(questionBank.Stage.NameEn, $"%{searchTerm}%"))))
            .WhereIf(
                request.QuestionBankTypeId.HasValue,
                questionBank => questionBank.QuestionBankTypeId == request.QuestionBankTypeId)
            .WhereIf(
                request.ManagementId.HasValue,
                questionBank => questionBank.ManagementId == request.ManagementId)
            .WhereIf(
                request.JobTitleId.HasValue,
                questionBank => questionBank.JobTitleId == request.JobTitleId)
            .WhereIf(request.StageId.HasValue, questionBank => questionBank.StageId == request.StageId)
            .WhereIf(request.IsActive.HasValue, questionBank => questionBank.IsActive == request.IsActive);

        var descending = string.Equals(request.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        var sorted = request.SortBy?.Trim().ToLowerInvariant() switch
        {
            "questionbanktype" => descending
                ? query.OrderByDescending(x => x.QuestionBankType.NameEn).ThenBy(x => x.Id)
                : query.OrderBy(x => x.QuestionBankType.NameEn).ThenBy(x => x.Id),
            "management" => descending
                ? query.OrderByDescending(x => x.Management == null ? string.Empty : x.Management.NameEn).ThenBy(x => x.Id)
                : query.OrderBy(x => x.Management == null ? string.Empty : x.Management.NameEn).ThenBy(x => x.Id),
            "jobtitle" => descending
                ? query.OrderByDescending(x => x.JobTitle == null ? string.Empty : x.JobTitle.JobNameEn).ThenBy(x => x.Id)
                : query.OrderBy(x => x.JobTitle == null ? string.Empty : x.JobTitle.JobNameEn).ThenBy(x => x.Id),
            "currentversion" => descending
                ? query.OrderByDescending(x => x.CurrentApprovedVersion == null ? 0 : x.CurrentApprovedVersion.VersionNo).ThenBy(x => x.Id)
                : query.OrderBy(x => x.CurrentApprovedVersion == null ? 0 : x.CurrentApprovedVersion.VersionNo).ThenBy(x => x.Id),
            "status" => descending
                ? query.OrderByDescending(x => x.IsActive).ThenBy(x => x.Id)
                : query.OrderBy(x => x.IsActive).ThenBy(x => x.Id),
            _ => query.OrderByDescending(x => x.UpdatedDate ?? x.CreatedDate).ThenBy(x => x.Id)
        };

        var questionBanks = await sorted
            .ToPaginatedListAsync<QuestionBank, QuestionBankListItemDto>(
                mapper,
                request with { SortBy = null },
                cancellationToken);

        return Result.Ok(questionBanks);
    }
}
