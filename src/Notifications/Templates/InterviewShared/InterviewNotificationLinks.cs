namespace Tawtheef.Notifications.Templates.InterviewShared;

/// <summary>Operations-portal paths used by the interview notification templates.</summary>
public static class InterviewNotificationLinks
{
    /// <summary>Where committee members conduct the evaluations of a schedule.</summary>
    public const string StartInterviewPath = "/portal/start-interview";

    /// <summary>The list page of each interview workflow item.</summary>
    public static string DefaultPath(InterviewEntityKind kind) => kind switch
    {
        InterviewEntityKind.Template => "/portal/interview-templates",
        InterviewEntityKind.Committee => "/portal/interview-committees",
        InterviewEntityKind.Schedule => "/portal/interviews",
        InterviewEntityKind.ResultReport => "/portal/approve-interview",
        _ => "/portal"
    };

    /// <summary>
    /// <paramref name="linkPath"/> when it is a relative portal path ("/..."), otherwise the kind's list page,
    /// so a missing or malformed path (or an absolute URL to another site) never ends up in a button.
    /// </summary>
    public static string Resolve(InterviewEntityKind kind, string? linkPath) => Resolve(linkPath, DefaultPath(kind));

    /// <summary><paramref name="linkPath"/> when it is a relative portal path ("/..."), otherwise <paramref name="fallback"/>.</summary>
    public static string Resolve(string? linkPath, string fallback) =>
        !string.IsNullOrWhiteSpace(linkPath) && linkPath.StartsWith('/') && !linkPath.StartsWith("//")
            ? linkPath
            : fallback;
}
