using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.InterviewResultReview;

public static class InterviewResultReview
{
    public const string TemplateKey = nameof(InterviewResultReview);
}

/// <summary>Asks the committee chair to review a newly generated interview result report and send it on to final approval.</summary>
/// <param name="ScheduleName">The interview schedule whose results these are.</param>
/// <param name="CandidateCount">Candidates in the report.</param>
/// <param name="LinkPath">Relative operations portal path to the chair's review screen.</param>
[NotificationTemplate(InterviewResultReview.TemplateKey, "نتائج المقابلات بانتظار مراجعتكم", "Interview results awaiting your review")]
public sealed record InterviewResultReviewModel(
    string ScheduleName,
    string? JobTitle,
    string? CommitteeName,
    int CandidateCount,
    string LinkPath);
