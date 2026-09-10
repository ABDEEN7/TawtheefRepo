using System.ComponentModel.DataAnnotations.Schema;
using FluentResults;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Interview.TemplateVersion;

namespace Tawtheef.Domain.Entities.Interview;

/// <summary>
/// Represents a version of an interview template, allowing for tracking changes and maintaining historical versions of interview templates.
/// </summary>
/// we seperate the version from the template because we want to keep the history of the template and its versions, and we want to be able to compare different versions of the same template.
/// so i make changes to the DB suggested design, so we need to jsut use the active version of the template, and we can keep the history of the template and its versions in the InterviewTemplateVersion table.

[Table(nameof(InterviewTemplateVersion), Schema = Schemas.Interview)]
public class InterviewTemplateVersion : EventEntity
{
    public Guid InterviewTemplateId { get; set; }
    public InterviewTemplate? InterviewTemplate { get; set; }
    public int VersionNo { get; set; }
    [Column(TypeName = "decimal(6,2)")]
    public decimal FinalScore { get; set; }
    [Column(TypeName = "decimal(6,2)")]
    public decimal? QualificationScore { get; set; }
    public CalculationMethod CalculationMethod { get; set; }
    public TemplateVersionStatus Status { get; set; } = TemplateVersionStatus.Draft;
    public bool IsLocked { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public Guid? ApprovedById { get; set; }
    public User? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? DecisionNotes { get; set; }
    // each version can have multiple axes, and each axis can have multiple criteria, so we need to keep the axes and criteria in the version table,
    // so we can keep the history of the axes and criteria for each version.
    public ICollection<InterviewTemplateEvaluationAxis> Axes { get; set; } = [];


    /// <summary>
    /// Creates a new instance of the InterviewTemplateVersion class with the specified parameters.
    /// we use this method to create a new version of the template, and we set the status to draft, and we set the islocked to false.
    /// </summary>
    /// <param name="interviewTemplateId"></param>
    /// <param name="versionNo"></param> its auto generated ver + 1 or 1 for initial ver
    /// <param name="finalScore"></param>
    /// <param name="qualificationScore"></param>
    /// <param name="calculationMethod"></param>
    /// <returns></returns>
    public static InterviewTemplateVersion Create(
        Guid interviewTemplateId, int versionNo, decimal finalScore,
        decimal? qualificationScore, CalculationMethod calculationMethod)
    {
        return new InterviewTemplateVersion
        {
            InterviewTemplateId = interviewTemplateId,
            VersionNo = versionNo,
            FinalScore = finalScore,
            QualificationScore = qualificationScore,
            CalculationMethod = calculationMethod,
            Status = TemplateVersionStatus.Draft,
            IsLocked = false
        };
    }

    // this method is used to ensure that the version is editable , and if it is not editable, it will return a failure result with the appropriate error code.
    // before edit axis or creiteria we need to ensure the version not locked (guard : bussinwss rule)
    // we locked vsersion when its approved or superseded, so we cann't edit it anymore (bussiness rule)
    // Set by the system the first time this version is bound to a job or scored against.
    // Never set by a user, and never reversed.
    public void Lock() => IsLocked = true;

    public Result EnsureEditable() =>
        Status is TemplateVersionStatus.Draft or TemplateVersionStatus.Returned
            ? Result.Ok()
            : Result.Fail(new Error(ErrorsCodes.InterviewTemplateVersionLocked));

    public Result UpdateScoring(decimal finalScore, decimal? qualificationScore, CalculationMethod calculationMethod)
    {
        var editable = EnsureEditable();
        if (editable.IsFailed)
            return editable;

        FinalScore = finalScore;
        QualificationScore = qualificationScore;
        CalculationMethod = calculationMethod;
        return Result.Ok();
    }

    public Result Submit()
    {
        if (Status is not (TemplateVersionStatus.Draft or TemplateVersionStatus.Returned))
            return Result.Fail(new Error(ErrorsCodes.InterviewTemplateVersionNotDraftOrReturned));

        Status = TemplateVersionStatus.PendingApproval;
        AddDomainEvent(new TemplateVersionSubmittedEvent(this, DateTimeOffset.Now));
        return Result.Ok();
    }

    public Result Approve(Guid approverId, string? decisionNotes)
    {
        if (Status != TemplateVersionStatus.PendingApproval)
            return Result.Fail(new Error(ErrorsCodes.InterviewTemplateVersionNotPendingApproval));

        Status = TemplateVersionStatus.Approved;
        ApprovedById = approverId;
        ApprovedAt = DateTime.UtcNow;
        DecisionNotes = decisionNotes;
        AddDomainEvent(new TemplateVersionApprovedEvent(this, DateTimeOffset.Now));
        return Result.Ok();
    }

    // Only one version per template can be Approved at a time (DB-enforced by a unique filtered index),
    // so approving a new version must supersede whichever version currently holds that spot.
    public Result Supersede()
    {
        if (Status != TemplateVersionStatus.Approved)
            return Result.Fail(new Error(ErrorsCodes.InterviewTemplateVersionNotApproved));

        Status = TemplateVersionStatus.Superseded;
        AddDomainEvent(new TemplateVersionSupersededEvent(this, DateTimeOffset.Now));
        return Result.Ok();
    }

    public Result Return(string reason)
    {
        if (Status != TemplateVersionStatus.PendingApproval)
            return Result.Fail(new Error(ErrorsCodes.InterviewTemplateVersionNotPendingApproval));

        Status = TemplateVersionStatus.Returned;
        DecisionNotes = reason;
        AddDomainEvent(new TemplateVersionReturnedEvent(this, DateTimeOffset.Now));
        return Result.Ok();
    }

    public Result Cancel(string reason)
    {
        if (Status is TemplateVersionStatus.Approved or TemplateVersionStatus.Cancelled
            or TemplateVersionStatus.Superseded)
            return Result.Fail(new Error(ErrorsCodes.InterviewTemplateVersionNotCancellable));

        Status = TemplateVersionStatus.Cancelled;
        DecisionNotes = reason;
        AddDomainEvent(new TemplateVersionCancelledEvent(this, DateTimeOffset.Now));
        return Result.Ok();
    }

    // Adding an axis needs the sibling list in memory to enforce "no duplicate axis per version",
    public Result<InterviewTemplateEvaluationAxis> AddAxis(
        Guid interviewEvaluationAxisId, decimal maxScore, decimal? qualificationScore, int orderNo)
    {
        var editable = EnsureEditable();
        if (editable.IsFailed)
            return Result.Fail<InterviewTemplateEvaluationAxis>(editable.Errors);

        if (Axes.Any(a => a.InterviewEvaluationAxisId == interviewEvaluationAxisId))
            return Result.Fail<InterviewTemplateEvaluationAxis>(new Error(ErrorsCodes.InterviewTemplateVersionAxisAlreadyExists));

        var axis = new InterviewTemplateEvaluationAxis
        {
            InterviewTemplateVersionId = Id,
            InterviewEvaluationAxisId = interviewEvaluationAxisId,
            MaxScore = maxScore,
            QualificationScore = qualificationScore,
            OrderNo = orderNo
        };

        Axes.Add(axis);
        return Result.Ok(axis);
    }
}

public enum CalculationMethod
{
    AverageOfEvaluators = 1,
    WeightedByRole = 2,
    ChairmanDecides = 3
}

public enum TemplateVersionStatus
{
    Draft = 1,
    PendingApproval = 2,
    Returned = 3,
    Approved = 4,
    Superseded = 5,
    Cancelled = 6
}
