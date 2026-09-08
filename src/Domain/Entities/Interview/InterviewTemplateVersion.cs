using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

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
