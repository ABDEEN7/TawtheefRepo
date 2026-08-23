using Application.Operation.Features.Employee.Dashboard.DTOs.Candidates;
using Application.Operation.Features.Employee.Dashboard.Services.Access;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Configurations.Rules;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.MinisterOffice;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Dashboard.Services.Read;

internal sealed class DashboardProfileMetricsReader(
    IUnitOfWork uow,
    ILocalizationService localizationService)
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
        var now = DateTime.UtcNow;
        var newProfiles = await CountNewProfilesAsync(profiles, now, ct);
        var averageApprovalHours = await CalculateAverageApprovalHoursAsync(
            profiles, range.From, range.To, ct);
        var cohorts = await GetCohortsAsync(
            profiles,
            context.Scope == DashboardScope.Organization,
            ct);
        var followedMinisterOfficeCandidates = context.Scope == DashboardScope.Organization
            ? await uow.GetEntityRepository<MinisterOfficeCandidate>()
                .DbSet.CountAsync(candidate => !candidate.IsDeleted && candidate.IsFollowUpActive, ct)
            : 0;

        return new DashboardProfileMetrics(
            candidateTypes,
            statuses,
            unassigned,
            rejected,
            newProfiles.Today,
            newProfiles.ThisWeek,
            newProfiles.ThisMonth,
            averageApprovalHours,
            followedMinisterOfficeCandidates,
            cohorts);
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

        return rows
            .OrderByDescending(row => row.CandidateTypeId == CandidateTypeIds.Qatari)
            .ThenByDescending(row => row.Count)
            .ThenBy(row => row.NameEn)
            .Select(row => new CandidateTypeCountDto
            {
                CandidateTypeId = row.CandidateTypeId,
                Label = localizationService.GetLocalizedValue(
                    row.NameAr,
                    row.NameEn),
                Count = row.Count
            })
            .ToList();
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

    private static async Task<(int Today, int ThisWeek, int ThisMonth)> CountNewProfilesAsync(
        IQueryable<UserProfile> profiles,
        DateTime now,
        CancellationToken ct)
    {
        var today = now.Date;
        var weekStart = GetWeekStart(today);
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var earliestStart = new[] { today, weekStart, monthStart }.Min();

        var counts = await profiles
            .Where(profile => profile.CreatedDate >= earliestStart && profile.CreatedDate < now)
            .GroupBy(_ => 1)
            .Select(group => new
            {
                Today = group.Count(profile => profile.CreatedDate >= today),
                ThisWeek = group.Count(profile => profile.CreatedDate >= weekStart),
                ThisMonth = group.Count(profile => profile.CreatedDate >= monthStart)
            })
            .FirstOrDefaultAsync(ct);

        return counts is null
            ? (0, 0, 0)
            : (counts.Today, counts.ThisWeek, counts.ThisMonth);
    }

    private static async Task<decimal> CalculateAverageApprovalHoursAsync(
        IQueryable<UserProfile> profiles,
        DateTime from,
        DateTime to,
        CancellationToken ct)
    {
        var approvals = profiles.Where(profile => profile.CreatedDate >= from &&
            profile.CreatedDate < to && profile.Status == UserProfileStatus.Approved &&
            profile.UpdatedDate != null);
        var hours = await approvals
            .Select(profile => (double?)(EF.Functions.DateDiffSecond(
                profile.CreatedDate,
                profile.UpdatedDate!.Value) / 3600.0))
            .AverageAsync(ct);
        return hours.HasValue ? Math.Round((decimal)hours.Value, 2) : 0m;
    }

    private async Task<CandidateCohortsDto> GetCohortsAsync(
        IQueryable<UserProfile> profiles,
        bool includeOfficeProfiles,
        CancellationToken ct)
    {
        var ministerOfficeCandidates = uow.GetEntityRepository<MinisterOfficeCandidate>()
            .DbSet.AsNoTracking();

        return await profiles
            .GroupBy(_ => 1)
            .Select(group => new CandidateCohortsDto
            {
                IncludeOfficeProfiles = includeOfficeProfiles,
                RegisteredKawaderProfiles = group.Count(profile =>
                    profile.User != null && profile.User.IsUserKawader),
                RegisteredMinisterOfficeProfiles = group.Count(profile =>
                    profile.NationalNumber != null && ministerOfficeCandidates.Any(candidate =>
                        !candidate.IsDeleted && candidate.IsFollowUpActive &&
                        candidate.Qid == profile.NationalNumber)),
                QatarGraduateProfiles = group.Count(profile => profile.Qualifications!.Any(qualification =>
                    !qualification.IsDeleted && qualification.CountryId == CountryIds.Qatar)),
                OfficeProfiles = group.Count(profile => profile.OfficeId.HasValue)
            })
            .FirstOrDefaultAsync(ct) ?? new CandidateCohortsDto
            {
                IncludeOfficeProfiles = includeOfficeProfiles
            };
    }

    private static Dictionary<string, int> ToStatusDictionary(IEnumerable<(string Status, int Count)> rows) =>
        rows.ToDictionary(row => row.Status, row => row.Count, StringComparer.OrdinalIgnoreCase);

    private static DateTime GetWeekStart(DateTime today) => today.AddDays(-(int)today.DayOfWeek);
}
