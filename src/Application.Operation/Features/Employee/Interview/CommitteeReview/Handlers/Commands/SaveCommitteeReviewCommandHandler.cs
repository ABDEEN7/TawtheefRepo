using System.Text.Json;
using Application.Operation.Features.Employee.Interview.CommitteeReview.Commands;
using Application.Operation.Features.Employee.Interview.Evaluation.Services;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.Interview.CommitteeReview.Handlers.Commands;

// Chair (or HR bypass) only. Changing a candidate's recommended decision additionally needs the
// InterviewCommitteeReview.OverrideSuggestion permission - checked here, not only by the disabled
// dropdown. The school stage is informational and never touches scores, qualification or suggestion.
public sealed class SaveCommitteeReviewCommandHandler(IUnitOfWork unitOfWork, EvaluationAccessResolver accessResolver)
    : IRequestHandler<SaveCommitteeReviewCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveCommitteeReviewCommand request, CancellationToken cancellationToken)
    {
        var report = await unitOfWork.GetEntityRepository<InterviewResultReport>().DbSet
            .Include(r => r.Candidates).ThenInclude(c => c.InterviewAppointment)
            .FirstOrDefaultAsync(r => r.Id == request.ReportId, cancellationToken);
        if (report is null || report.Candidates.Count == 0)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewResultReportNotFound));

        var committeeId = report.Candidates.First().InterviewAppointment!.InterviewCommitteeId;
        if (!await accessResolver.IsChairOrBypassAsync(committeeId, cancellationToken))
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewCommitteeReviewForbidden)
                .WithMetadata("StatusCode", StatusCodes.Status403Forbidden));

        if (!report.IsCommitteeReviewEditable)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewResultReportNotInCommitteeReview));

        var canOverride = accessResolver.HasPermission(PermissionKeys.InterviewCommitteeReview.OverrideSuggestion);

        var stageIds = request.Candidates.Where(c => c.SchoolStageId.HasValue).Select(c => c.SchoolStageId!.Value).Distinct().ToList();
        if (stageIds.Count > 0)
        {
            var activeStageCount = await unitOfWork.GetEntityRepository<SchoolStage>().DbSet
                .CountAsync(s => stageIds.Contains(s.Id) && s.IsActive, cancellationToken);
            if (activeStageCount != stageIds.Count)
                return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewSchoolStageNotFound));
        }

        foreach (var input in request.Candidates)
        {
            var candidate = report.Candidates.FirstOrDefault(c => c.Id == input.CandidateId);
            if (candidate is null)
                return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewResultReportNotFound));

            var reason = input.RecommendedDecision is null ? null : input.Reason?.Trim();
            if (string.IsNullOrEmpty(reason))
                reason = null;

            // The client always sends every candidate, so only an actual change needs the permission.
            var recommendationChanged = candidate.ChairRecommendedDecision != input.RecommendedDecision
                || candidate.ChairRecommendationReason != reason;
            if (recommendationChanged)
            {
                if (!canOverride)
                    return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewCommitteeReviewOverrideForbidden)
                        .WithMetadata("StatusCode", StatusCodes.Status403Forbidden));

                var recommendResult = candidate.SetChairRecommendation(input.RecommendedDecision, reason);
                if (recommendResult.IsFailed)
                    return Result.Fail<Unit>(recommendResult.Errors);
            }

            var stageResult = candidate.SetRecommendedSchoolStage(input.SchoolStageId);
            if (stageResult.IsFailed)
                return Result.Fail<Unit>(stageResult.Errors);
        }

        if (request.SendForApproval)
        {
            var sendResult = report.SendForApproval(accessResolver.GetCurrentUserId() ?? Guid.Empty);
            if (sendResult.IsFailed)
                return Result.Fail<Unit>(sendResult.Errors);
        }

        await unitOfWork.GetEntityRepository<InterviewAuditLog>().AddAsync(new InterviewAuditLog
        {
            EntityType = nameof(InterviewResultReport),
            EntityId = report.Id,
            Action = request.SendForApproval
                ? InterviewResultReportAuditActions.SentForApproval
                : InterviewResultReportAuditActions.CommitteeReviewSaved,
            NewValues = JsonSerializer.Serialize(new
            {
                request.ReportId,
                Candidates = report.Candidates.Select(c => new
                {
                    CandidateId = c.Id,
                    c.InterviewAppointmentId,
                    c.ChairRecommendedDecision,
                    c.ChairRecommendationReason,
                    c.RecommendedSchoolStageId
                })
            })
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
