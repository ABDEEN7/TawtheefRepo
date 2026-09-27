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

namespace Application.Operation.Features.Employee.QuestionBankAssignments.Handlers;

internal static class QuestionEntryRules
{
    public static bool Editable(Guid status) => status == QuestionBankAssignmentStatusIds.Assigned || status == QuestionBankAssignmentStatusIds.QuestionEntryInProgress;
    public static bool Valid(QuestionInput q)
    {
        if (q.QuestionTypeId != QuestionTypeIds.MULTIPLE_CHOICE && q.QuestionTypeId != QuestionTypeIds.TRUE_FALSE) return false;
        if (q.DifficultyLevelId != DifficultyLevelIds.EASY && q.DifficultyLevelId != DifficultyLevelIds.MEDIUM && q.DifficultyLevelId != DifficultyLevelIds.HARD) return false;
        if (string.IsNullOrWhiteSpace(q.QuestionTextAr) && string.IsNullOrWhiteSpace(q.QuestionTextEn)) return false;
        if (q.Options.Count < 2 || q.Options.Count(x => x.IsCorrect) != 1 || q.Options.Select(x => x.DisplayOrder).Distinct().Count() != q.Options.Count) return false;
        if (q.Options.Any(x => string.IsNullOrWhiteSpace(x.OptionTextAr) && string.IsNullOrWhiteSpace(x.OptionTextEn))) return false;
        return q.QuestionTypeId != QuestionTypeIds.TRUE_FALSE || q.Options.Count == 2;
    }
    public static QuestionRevision Revision(Guid questionId, int number, Guid? itemId, QuestionInput q) => new()
    {
        Id = Guid.NewGuid(), QuestionId = questionId, RevisionNo = number, SourceRequestItemId = itemId,
        QuestionTypeId = q.QuestionTypeId, DifficultyLevelId = q.DifficultyLevelId,
        QuestionTextAr = q.QuestionTextAr?.Trim(), QuestionTextEn = q.QuestionTextEn?.Trim(),
        ExplanationAr = q.ExplanationAr?.Trim(), ExplanationEn = q.ExplanationEn?.Trim(),
        Options = q.Options.OrderBy(x => x.DisplayOrder).Select(x => new QuestionRevisionOption
        {
            Id = Guid.NewGuid(), OptionTextAr = x.OptionTextAr?.Trim(), OptionTextEn = x.OptionTextEn?.Trim(),
            IsCorrect = x.IsCorrect, DisplayOrder = x.DisplayOrder
        }).ToList()
    };
}
