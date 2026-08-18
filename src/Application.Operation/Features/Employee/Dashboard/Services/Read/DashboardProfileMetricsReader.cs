using Application.Operation.Features.Employee.Dashboard.DTOs.Candidates;
using Application.Operation.Features.Employee.Dashboard.Services.Access;
using Application.Operation.Features.Employee.Dashboard.Services.Scopes;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Configurations.Rules;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Dashboard.Services.Read;

internal sealed class DashboardProfileMetricsReader(
    IUnitOfWork uow,
    ILocalizationService localizationService,
    DashboardQueryScope scope)
{
    public async Task<DashboardProfileMetrics> ReadAsync(
        IQueryable<UserProfile> profiles,
        IQueryable<UserProfile> periodProfiles,
        DashboardAccessContext context,
        DateTime currentFrom,
        (DateTime From, DateTime To) range,
        CancellationToken ct)
    {
        var candidateTypes = await GetCandidateTypesAsync(profiles, ct);
        var statuses = await GetPeriodStatusesAsync(periodProfiles, currentFrom, ct);
        var unassigned = await CountUnassignedByPeriodAsync(periodProfiles, currentFrom, ct);
        var rejected = await CountRejectedAsync(profiles, range.From, range.To, ct);
        var accessibleProfiles = scope.AccessibleProfiles(context);
        var now = DateTime.UtcNow;
        var newToday = await CountNewProfilesAsync(accessibleProfiles, now.Date, now, ct);
        var newThisWeek = await CountNewProfilesAsync(accessibleProfiles, GetWeekStart(now.Date), now, ct);
        var newThisMonth = await CountNewProfilesAsync(
            accessibleProfiles, new DateTime(now.Year, now.Month, 1), now, ct);
        var averageApprovalHours = await CalculateAverageApprovalHoursAsync(
            accessibleProfiles, range.From, range.To, ct);
        var followedMinisterOfficeCandidates = context.CanViewMinisterOffice
            ? await uow.GetEntityRepository<Tawtheef.Domain.Entities.MinisterOffice.MinisterOfficeCandidate>()
                .DbSet.CountAsync(candidate => !candidate.IsDeleted && candidate.IsFollowUpActive, ct)
            : 0;

        return new DashboardProfileMetrics(
            candidateTypes,
            statuses,
            unassigned,
            rejected,
            newToday,
            newThisWeek,
            newThisMonth,
            averageApprovalHours,
            followedMinisterOfficeCandidates);
    }

    private async Task<List<CandidateTypeCountDto>> GetCandidateTypesAsync(
        IQueryable<UserProfile> profiles,
        CancellationToken ct)
    {
        var rows = await profiles
            .Where(profile =>
                profile.CandidateTypeId.HasValue &&
                profile.CandidateType != null)
            .GroupBy(profile => new
            {
                CandidateTypeId = profile.CandidateTypeId!.Value,
                profile.CandidateType!.NameAr,
                profile.CandidateType.NameEn
            })
            .Select(group => new
            {
                group.Key.CandidateTypeId,
                group.Key.NameAr,
                group.Key.NameEn,
                Count = group.Count()
            })
            .ToListAsync(ct);

        return rows.Select(row => new CandidateTypeCountDto
        {
            CandidateTypeId = row.CandidateTypeId,
            Label = localizationService.GetLocalizedValue(
                row.NameAr,
                row.NameEn),
            Count = row.Count
        }).ToList();
    }

    private static async Task<PeriodValues<Dictionary<string, int>>> GetPeriodStatusesAsync(
        IQueryable<UserProfile> profiles,
        DateTime currentFrom,
        CancellationToken ct)
    {
        var rows = await profiles
            .GroupBy(profile => new { IsCurrent = profile.CreatedDate >= currentFrom, profile.Status })
            .Select(group => new
            {
                group.Key.IsCurrent,
                Status = group.Key.Status.ToString(),
                Count = group.Count()
            })
            .ToListAsync(ct);
        return new PeriodValues<Dictionary<string, int>>(
            ToStatusDictionary(rows.Where(row => row.IsCurrent).Select(row => (row.Status, row.Count))),
            ToStatusDictionary(rows.Where(row => !row.IsCurrent).Select(row => (row.Status, row.Count))));
    }

    private async Task<PeriodValues<int>> CountUnassignedByPeriodAsync(
        IQueryable<UserProfile> profiles,
        DateTime currentFrom,
        CancellationToken ct)
    {
        var assignments = uow.GetEntityRepository<ProfileAssignment>().DbSet;
        var changes = uow.GetEntityRepository<ProfileChangeRequest>().DbSet.AsNoTracking();
        var rows = await profiles
            .Where(profile =>
                (Enumerable.Contains(ProfileDistributionRules.AssignableStatuses, profile.Status) ||
                 profile.Status == UserProfileStatus.Approved && changes.Any(change =>
                     change.UserProfileId == profile.Id &&
                     (change.Status == ProfileChangeRequestStatus.Pending ||
                      change.Status == ProfileChangeRequestStatus.UnderReview))) &&
                !assignments.Any(assignment => assignment.UserProfileId == profile.Id &&
                    assignment.IsActive && assignment.UnassignedAtUtc == null))
            .GroupBy(profile => profile.CreatedDate >= currentFrom)
            .Select(group => new { IsCurrent = group.Key, Count = group.Count() })
            .ToListAsync(ct);
        return new PeriodValues<int>(
            rows.FirstOrDefault(row => row.IsCurrent)?.Count ?? 0,
            rows.FirstOrDefault(row => !row.IsCurrent)?.Count ?? 0);
    }

    private Task<int> CountRejectedAsync(
        IQueryable<UserProfile> profiles,
        DateTime from,
        DateTime to,
        CancellationToken ct)
    {
        var allowedProfiles = profiles.Select(profile => profile.Id);
        return uow.GetEntityRepository<ReviewItem>().DbSet.AsNoTracking()
            .Where(item => item.Status == ReviewStatus.Rejected && item.UserProfile != null &&
                           allowedProfiles.Contains(item.UserProfileId) &&
                           item.UserProfile.CreatedDate >= from && item.UserProfile.CreatedDate < to)
            .Select(item => item.UserProfileId).Distinct().CountAsync(ct);
    }

    private static Task<int> CountNewProfilesAsync(
        IQueryable<UserProfile> profiles,
        DateTime from,
        DateTime to,
        CancellationToken ct) =>
        profiles.CountAsync(profile => profile.CreatedDate >= from && profile.CreatedDate < to, ct);

    private static async Task<decimal> CalculateAverageApprovalHoursAsync(
        IQueryable<UserProfile> profiles,
        DateTime from,
        DateTime to,
        CancellationToken ct)
    {
        var approvals = profiles.Where(profile => profile.CreatedDate >= from &&
            profile.CreatedDate < to && profile.Status == UserProfileStatus.Approved &&
            profile.UpdatedDate != null);
        var count = await approvals.CountAsync(ct);
        if (count == 0) return 0m;
        var hours = await approvals.Select(profile =>
            EF.Functions.DateDiffSecond(profile.CreatedDate, profile.UpdatedDate!.Value) / 3600.0)
            .SumAsync(ct);
        return Math.Round((decimal)(hours / count), 2);
    }

    private static Dictionary<string, int> ToStatusDictionary(IEnumerable<(string Status, int Count)> rows) =>
        rows.ToDictionary(row => row.Status, row => row.Count, StringComparer.OrdinalIgnoreCase);

    private static DateTime GetWeekStart(DateTime today) => today.AddDays(-(int)today.DayOfWeek);
}
