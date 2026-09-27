using System.ComponentModel.DataAnnotations.Schema;
using FluentResults;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.Exams;

[Table(nameof(ExamResultReport), Schema = Schemas.Hr)]
public class ExamResultReport : EventEntity
{
    public Guid ExamId { get; set; }

    public decimal SpecializedQualificationThreshold { get; set; }

    public Guid StatusId { get; set; }

    public Guid? ApprovedById { get; set; }
    public DateTime? ApprovedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public string? DecisionNotes { get; set; }

    public Exam? Exam { get; set; }
    public ExamResultReportStatus? Status { get; set; }

    public ICollection<ExamResultCandidate> Candidates { get; set; }
        = new List<ExamResultCandidate>();

    public static ExamResultReport Create(
        Guid examId,
        decimal specializedQualificationThreshold)
    {
        return new ExamResultReport
        {
            ExamId = examId,
            SpecializedQualificationThreshold =
                specializedQualificationThreshold,
            StatusId = ExamResultReportStatusIds.Creating
        };
    }

    public Result MarkReadyForReview()
    {
        if (StatusId != ExamResultReportStatusIds.Creating)
        {
            return Result.Fail(
                new Error(
                    ErrorsCodes.ExamResultReportNotCreating));
        }

        StatusId = ExamResultReportStatusIds.UnderReview;

        return Result.Ok();
    }

    public Result ReturnForProcessing(string reason)
    {
        if (StatusId != ExamResultReportStatusIds.UnderReview)
        {
            return Result.Fail(
                new Error(
                    ErrorsCodes.ExamResultReportNotUnderReview));
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            return Result.Fail(
                new Error(
                    ErrorsCodes.ExamResultReportReturnReasonRequired));
        }

        StatusId = ExamResultReportStatusIds.Returned;
        DecisionNotes = reason.Trim();

        return Result.Ok();
    }

    public Result MarkProcessingCompleted()
    {
        if (StatusId != ExamResultReportStatusIds.Returned)
        {
            return Result.Fail(
                new Error(ErrorsCodes.ExamResultReportNotReturned));
        }

        StatusId = ExamResultReportStatusIds.UnderReview;
        return Result.Ok();
    }

    public Result Approve(
        Guid approverId,
        string? decisionNotes)
    {
        if (StatusId != ExamResultReportStatusIds.UnderReview)
        {
            return Result.Fail(
                new Error(
                    ErrorsCodes.ExamResultReportNotUnderReview));
        }

        if (Candidates.Any(x =>
                x.ResultStatusId ==
                    ExamResultCandidateResultStatusIds
                        .RequiresAction))
        {
            return Result.Fail(
                new Error(
                    ErrorsCodes
                        .ExamResultReportHasPendingActions));
        }

        StatusId = ExamResultReportStatusIds.Approved;

        ApprovedById = approverId;
        ApprovedAt = DateTime.UtcNow;

        DecisionNotes =
            string.IsNullOrWhiteSpace(decisionNotes)
                ? null
                : decisionNotes.Trim();

        return Result.Ok();
    }

    public Result Close()
    {
        if (StatusId != ExamResultReportStatusIds.Approved)
        {
            return Result.Fail(
                new Error(
                    ErrorsCodes.ExamResultReportNotApproved));
        }

        StatusId = ExamResultReportStatusIds.Closed;
        ClosedAt = DateTime.UtcNow;

        return Result.Ok();
    }
}
