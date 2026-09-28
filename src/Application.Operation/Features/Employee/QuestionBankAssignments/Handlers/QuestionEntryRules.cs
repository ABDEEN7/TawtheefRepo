using Application.Operation.Features.Employee.QuestionBankAssignments.Commands;
using Application.Operation.Features.Employee.QuestionBankAssignments.DTOs;
using Application.Operation.Features.Employee.QuestionBankAssignments.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Entities;
using Application.Operation.Features.Employee.QuestionBankAssignments.Services;

namespace Application.Operation.Features.Employee.QuestionBankAssignments.Handlers;

internal static class QuestionEntryRules
{
    private static readonly HashSet<string> ImageTypes = new(StringComparer.OrdinalIgnoreCase)
        { "image/jpeg", "image/png", "image/webp" };

    public static async Task<bool> ValidResource(IUnitOfWork uow, Guid? resourceId, CancellationToken ct)
    {
        if (!resourceId.HasValue) return true;
        return await uow.GetEntityRepository<Resource>().DbSet.AsNoTracking()
            .AnyAsync(x => x.Id == resourceId && ImageTypes.Contains(x.Type) && x.Size > 0, ct);
    }
    public static bool Editable(Guid status) => status == QuestionBankAssignmentStatusIds.Assigned || status == QuestionBankAssignmentStatusIds.QuestionEntryInProgress;
    public static bool Valid(QuestionInput q)
    {
        if (q.QuestionTypeId != QuestionTypeIds.MULTIPLE_CHOICE && q.QuestionTypeId != QuestionTypeIds.TRUE_FALSE) return false;
        if (q.DifficultyLevelId != DifficultyLevelIds.EASY && q.DifficultyLevelId != DifficultyLevelIds.MEDIUM && q.DifficultyLevelId != DifficultyLevelIds.HARD) return false;
        var hasAr = RichContentPolicy.HasMeaningfulContent(RichContentPolicy.Sanitize(q.QuestionTextAr));
        var hasEn = RichContentPolicy.HasMeaningfulContent(RichContentPolicy.Sanitize(q.QuestionTextEn));
        if (!((hasAr && hasEn) || (!hasAr && !hasEn && q.ResourceId.HasValue))) return false;
        if (q.Options.Count < 2 || q.Options.Count(x => x.IsCorrect) != 1 || q.Options.Select(x => x.DisplayOrder).Distinct().Count() != q.Options.Count) return false;
        if (q.Options.Any(x => !RichContentPolicy.HasMeaningfulContent(RichContentPolicy.Sanitize(x.OptionTextAr)) ||
                               !RichContentPolicy.HasMeaningfulContent(RichContentPolicy.Sanitize(x.OptionTextEn)))) return false;
        return q.QuestionTypeId != QuestionTypeIds.TRUE_FALSE || q.Options.Count == 2;
    }
    public static QuestionRevision Revision(Guid questionId, int number, Guid? itemId, QuestionInput q) => new()
    {
        Id = Guid.NewGuid(), QuestionId = questionId, RevisionNo = number, SourceRequestItemId = itemId,
        QuestionTypeId = q.QuestionTypeId, DifficultyLevelId = q.DifficultyLevelId,
        QuestionTextAr = RichContentPolicy.Sanitize(q.QuestionTextAr), QuestionTextEn = RichContentPolicy.Sanitize(q.QuestionTextEn),
        ExplanationAr = RichContentPolicy.Sanitize(q.ExplanationAr), ExplanationEn = RichContentPolicy.Sanitize(q.ExplanationEn),
        ResourceId = q.ResourceId,
        Options = q.Options.OrderBy(x => x.DisplayOrder).Select(x => new QuestionRevisionOption
        {
            Id = Guid.NewGuid(), OptionTextAr = RichContentPolicy.Sanitize(x.OptionTextAr), OptionTextEn = RichContentPolicy.Sanitize(x.OptionTextEn),
            IsCorrect = x.IsCorrect, DisplayOrder = x.DisplayOrder
        }).ToList()
    };
}
