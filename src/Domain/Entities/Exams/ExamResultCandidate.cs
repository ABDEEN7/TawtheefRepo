using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Domain.Entities.Exams;

[Table(nameof(ExamResultCandidate), Schema = Schemas.Hr)]
public class ExamResultCandidate : EventEntity
{
    public Guid ExamResultReportId { get; set; }
    public Guid InvitationId { get; set; }
    public Guid? TestAttemptId { get; set; }
    public decimal? SpecializedScore { get; set; }
    public decimal? EducationalScore { get; set; }
    public decimal? SkillsScore { get; set; }
    public decimal? FinalScore { get; set; }
    public int? Rank { get; set; }
    public bool IsQualified { get; set; }
    public Guid ResultStatusId { get; set; }
    public DateTime SnapshotAt { get; set; }

    public ExamResultReport? ExamResultReport { get; set; }
    public Invitation? Invitation { get; set; }
    public TestAttempt? TestAttempt { get; set; }
    public ExamResultCandidateResultStatus? ResultStatus { get; set; }
}
