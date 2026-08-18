using Application.Operation.Features.Employee.Dashboard.DTOs.Jobs;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Dashboard.Services.Read;

internal sealed class DashboardJobMetricsReader(
    ILocalizationService localizationService)
{
    public async Task<DashboardJobMetrics> ReadAsync(
        IQueryable<Job> jobs,
        DateTime currentFrom,
        CancellationToken ct)
    {
        var rows = await jobs
            .GroupBy(job => new
            {
                IsCurrent = job.CreatedDate >= currentFrom,
                job.JobStatusId,
                NameAr = job.JobStatus != null
                    ? job.JobStatus.NameAr
                    : string.Empty,
                NameEn = job.JobStatus != null
                    ? job.JobStatus.NameEn
                    : string.Empty
            })
            .Select(group => new
            {
                group.Key.IsCurrent,
                group.Key.JobStatusId,
                group.Key.NameAr,
                group.Key.NameEn,
                Count = group.Count()
            })
            .ToListAsync(ct);
        var statuses = new PeriodValues<Dictionary<string, int>>(
            ToStatusDictionary(
                rows.Where(row => row.IsCurrent)
                    .Select(row => (row.JobStatusId.ToString(), row.Count))),
            ToStatusDictionary(
                rows.Where(row => !row.IsCurrent)
                    .Select(row => (row.JobStatusId.ToString(), row.Count))));
        var currentStatusBreakdown = rows
            .Where(row => row.IsCurrent)
            .Select(row => new JobStatusCountDto
            {
                JobStatusId = row.JobStatusId,
                Label = localizationService.GetLocalizedValue(
                    row.NameAr,
                    row.NameEn),
                Count = row.Count
            })
            .ToList();
        return new DashboardJobMetrics(
            statuses,
            new PeriodValues<JobKpisDto>(
                BuildKpis(statuses.Current),
                BuildKpis(statuses.Previous)),
            currentStatusBreakdown);
    }

    private static Dictionary<string, int> ToStatusDictionary(IEnumerable<(string Status, int Count)> rows) =>
        rows.ToDictionary(row => row.Status, row => row.Count, StringComparer.OrdinalIgnoreCase);

    private static JobKpisDto BuildKpis(IReadOnlyDictionary<string, int> statuses) => new()
    {
        TotalJobs = statuses.Values.Sum(),
        DraftJobs = statuses.GetValueOrDefault(JobStatusIds.Draft.ToString()),
        ActiveJobs = statuses.GetValueOrDefault(JobStatusIds.Published.ToString()),
        PendingReviewJobs = statuses.GetValueOrDefault(JobStatusIds.PendingApproval.ToString()),
        ApprovedJobs = statuses.GetValueOrDefault(JobStatusIds.PendingPointConfiguration.ToString()) +
                       statuses.GetValueOrDefault(JobStatusIds.NeedPointUpdate.ToString()) +
                       statuses.GetValueOrDefault(JobStatusIds.PendingPointApproval.ToString()),
        RejectedJobs = statuses.GetValueOrDefault(JobStatusIds.Rejected.ToString()),
        PendingPointConfigurationJobs = statuses.GetValueOrDefault(JobStatusIds.PendingPointConfiguration.ToString()),
        NeedPointUpdateJobs = statuses.GetValueOrDefault(JobStatusIds.NeedPointUpdate.ToString()),
        PendingPointApprovalJobs = statuses.GetValueOrDefault(JobStatusIds.PendingPointApproval.ToString()),
        NeedUpdateJobs = statuses.GetValueOrDefault(JobStatusIds.NeedUpdate.ToString()),
        ReadyForAnnouncementJobs = statuses.GetValueOrDefault(JobStatusIds.ReadyForAnnouncement.ToString()),
        PublishedJobs = statuses.GetValueOrDefault(JobStatusIds.Published.ToString()),
        ClosedJobs = statuses.GetValueOrDefault(JobStatusIds.Closed.ToString()),
        CancelledJobs = statuses.GetValueOrDefault(JobStatusIds.Cancelled.ToString())
    };
}
