using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Notifications.Templates.InterviewShared;

namespace Application.Operation.Common.Services.Interview;

/// <summary>What the approval notifications show about a template version / committee / schedule, in both languages.</summary>
public sealed record InterviewWorkflowItem(
    InterviewEntityKind Kind, string NameAr, string? NameEn, string? JobAr, string? JobEn, string LinkPath)
{
    public string Name(string lang) => InterviewNotificationText.Pick(lang, NameAr, NameEn);

    public string? Job(string lang) => JobAr is null ? null : InterviewNotificationText.Pick(lang, JobAr, JobEn);
}

/// <summary>
/// Builds <see cref="InterviewWorkflowItem"/>s. Names come from the in-memory entity carried by the event
/// (it may not be saved yet); only the job title is read from the database.
/// </summary>
public static class InterviewWorkflowItems
{
    public static async Task<InterviewWorkflowItem?> ForTemplateVersionAsync(
        IUnitOfWork unitOfWork, InterviewTemplateVersion version, CancellationToken ct)
    {
        var template = await unitOfWork.GetEntityRepository<InterviewTemplate>().DbSet
            .AsNoTracking()
            .Where(t => t.Id == version.InterviewTemplateId)
            .Select(t => new
            {
                t.Id,
                t.TitleAr,
                t.TitleEn,
                JobAr = t.JobTitle == null ? null : t.JobTitle.JobNameAr,
                JobEn = t.JobTitle == null ? null : t.JobTitle.JobNameEn
            })
            .FirstOrDefaultAsync(ct);

        return template is null
            ? null
            : new InterviewWorkflowItem(
                InterviewEntityKind.Template,
                $"{template.TitleAr} (v{version.VersionNo})",
                template.TitleEn is null ? null : $"{template.TitleEn} (v{version.VersionNo})",
                template.JobAr,
                template.JobEn,
                $"{InterviewNotificationLinks.DefaultPath(InterviewEntityKind.Template)}?template={template.Id}");
    }

    public static async Task<InterviewWorkflowItem> ForCommitteeAsync(
        IUnitOfWork unitOfWork, InterviewCommittee committee, CancellationToken ct)
    {
        var (jobAr, jobEn) = await JobTitleAsync(unitOfWork, committee.JobId, ct);
        return new InterviewWorkflowItem(
            InterviewEntityKind.Committee,
            WithCode(committee.NameAr, committee.Code),
            committee.NameEn is null ? null : WithCode(committee.NameEn, committee.Code),
            jobAr,
            jobEn,
            $"{InterviewNotificationLinks.DefaultPath(InterviewEntityKind.Committee)}?committee={committee.Id}");
    }

    public static async Task<InterviewWorkflowItem> ForScheduleAsync(
        IUnitOfWork unitOfWork, InterviewSchedule schedule, CancellationToken ct)
    {
        var (jobAr, jobEn) = await JobTitleAsync(unitOfWork, schedule.JobId, ct);
        return new InterviewWorkflowItem(
            InterviewEntityKind.Schedule,
            schedule.TitleAr,
            schedule.TitleEn,
            jobAr,
            jobEn,
            $"{InterviewNotificationLinks.DefaultPath(InterviewEntityKind.Schedule)}?schedule={schedule.Id}");
    }

    // A result report is named after its schedule: its REP-INT code is assigned by the database during the
    // same save that dispatches the event, so it doesn't exist yet when the notification is built.
    public static async Task<InterviewWorkflowItem?> ForResultReportAsync(
        IUnitOfWork unitOfWork, InterviewResultReport report, CancellationToken ct)
    {
        var schedule = await unitOfWork.GetEntityRepository<InterviewSchedule>().DbSet
            .AsNoTracking()
            .Where(s => s.Id == report.InterviewScheduleId)
            .Select(s => new { s.TitleAr, s.TitleEn, s.JobId })
            .FirstOrDefaultAsync(ct);
        if (schedule is null) return null;

        var (jobAr, jobEn) = await JobTitleAsync(unitOfWork, schedule.JobId, ct);
        return new InterviewWorkflowItem(
            InterviewEntityKind.ResultReport,
            schedule.TitleAr,
            schedule.TitleEn,
            jobAr,
            jobEn,
            $"{InterviewNotificationLinks.DefaultPath(InterviewEntityKind.ResultReport)}?schedule={report.InterviewScheduleId}");
    }

    public static async Task<(string? Ar, string? En)> JobTitleAsync(IUnitOfWork unitOfWork, Guid jobId, CancellationToken ct)
    {
        var job = await unitOfWork.GetEntityRepository<Job>().DbSet
            .AsNoTracking()
            .Where(j => j.Id == jobId)
            .Select(j => new { Ar = j.JobTitle == null ? null : j.JobTitle.JobNameAr, En = j.JobTitle == null ? null : j.JobTitle.JobNameEn })
            .FirstOrDefaultAsync(ct);
        return (job?.Ar, job?.En);
    }

    /// <summary>Committee display name: "name (code)".</summary>
    public static string WithCode(string name, string? code) =>
        string.IsNullOrWhiteSpace(code) ? name : $"{name} ({code})";
}
