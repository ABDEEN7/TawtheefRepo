using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Evaluation.Services;

// Single source of truth for "resolve the committee's template's currently Approved version, with
// its axes/criteria" - was copy-pasted verbatim across SaveMemberEvaluationDraftCommandHandler,
// SubmitMemberEvaluationCommandHandler and GetMemberEvaluationFormQueryHandler; extracted so the
// three can never drift. The predicate (InterviewTemplateId + Status == Approved) is covered by the
// existing UX_TemplateVersion_Approved filtered unique index - confirmed via a DB-backed harness
// this query already seeks that index, so this is a dedup/maintainability fix, not a perf fix.
public static class EvaluationTemplateResolver
{
    public static async Task<Result<InterviewTemplateVersion>> GetApprovedVersionAsync(
        IUnitOfWork unitOfWork, Guid interviewTemplateId, CancellationToken cancellationToken)
    {
        var version = await unitOfWork.GetEntityRepository<InterviewTemplateVersion>().DbSet
            .AsNoTracking()
            .AsSplitQuery()
            .Include(v => v.Axes).ThenInclude(a => a.InterviewEvaluationAxis)
            .Include(v => v.Axes).ThenInclude(a => a.Criteria).ThenInclude(c => c.InterviewEvaluationCriterion)
            .FirstOrDefaultAsync(v => v.InterviewTemplateId == interviewTemplateId && v.Status == TemplateVersionStatus.Approved, cancellationToken);

        return version is null
            ? Result.Fail<InterviewTemplateVersion>(new Error(ErrorsCodes.InterviewTemplateHasNoApprovedVersion))
            : Result.Ok(version);
    }
}
