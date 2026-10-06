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
    public Guid? TestSessionCandidateId { get; set; }
    public Guid? TestAttemptId { get; set; }
    public decimal? SpecializedScore { get; set; }
    public decimal? EducationalScore { get; set; }
    public decimal? SkillsScore { get; set; }
    public decimal? FinalScore { get; set; }
    public int? Rank { get; set; }
    public Guid ResultStatusId { get; set; }
    public DateTime SnapshotAt { get; set; }

    public ExamResultReport? ExamResultReport { get; set; }
    public Invitation? Invitation { get; set; }
    public TestSessionCandidate? TestSessionCandidate { get; set; }
    public TestAttempt? TestAttempt { get; set; }
    public ExamResultCandidateResultStatus? ResultStatus { get; set; }

    public void MarkNoShow(DateTime snapshotAt)
    {
        TestAttemptId = null;

        SpecializedScore = null;
        EducationalScore = null;
        SkillsScore = null;
        FinalScore = null;

        Rank = null;

        ResultStatusId =
            ExamResultCandidateResultStatusIds.NoShow;

        SnapshotAt = snapshotAt;
    }

    public void ApplyCalculatedResult(
        Guid testAttemptId,
        decimal specializedScore,
        decimal? educationalScore,
        decimal? skillsScore,
        decimal? finalScore,
        bool isSpecializedQualified,
        DateTime snapshotAt)
    {
        TestAttemptId = testAttemptId;

        SpecializedScore = specializedScore;
        EducationalScore = educationalScore;
        SkillsScore = skillsScore;
        FinalScore = finalScore;

        Rank = null;

        ResultStatusId =
            isSpecializedQualified
                ? ExamResultCandidateResultStatusIds.QualifiedForInterview
                : ExamResultCandidateResultStatusIds.NotQualifiedForInterview;

        SnapshotAt = snapshotAt;
    }

    public void MarkNotCompleted(DateTime snapshotAt)
    {
        ResultStatusId =
            ExamResultCandidateResultStatusIds.NotCompleted;

        SnapshotAt = snapshotAt;
    }

    public void MoveToRescheduledAssignment(
        TestSessionCandidate replacement,
        DateTime snapshotAt)
    {
        TestSessionCandidateId = replacement.Id;
        TestSessionCandidate = replacement;

        TestAttemptId = null;
        TestAttempt = null;

        SpecializedScore = null;
        EducationalScore = null;
        SkillsScore = null;
        FinalScore = null;

        Rank = null;

        SnapshotAt = snapshotAt;
    }
}
