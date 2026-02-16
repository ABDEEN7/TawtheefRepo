using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.ProfileApproval;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Handlers.Queries;

public sealed class GetProfileApprovalsHandler(IUnitOfWork uow, ILocalizationService localization)
    : IQueryHandler<GetProfileApprovalsQuery, Result<PaginatedResult<ProfileApprovalListItemDto>>>
{
    public async Task<Result<PaginatedResult<ProfileApprovalListItemDto>>> Handle(GetProfileApprovalsQuery request, CancellationToken ct)
    {
        // Guard clauses
        if (request.OfficerId == Guid.Empty)
            return Result.Fail<PaginatedResult<ProfileApprovalListItemDto>>(ErrorsCodes.InvalidUserIdentifier);

        // 1) Active assignments -> profile ids
        var assignedProfileIds = await GetAssignedProfileIdsAsync(request.OfficerId, ct);
        if (assignedProfileIds.Count == 0)
            return Result.Ok(PaginatedResult<ProfileApprovalListItemDto>.Empty);

        // 2) Load profiles (only relevant statuses) + apply DB pagination
        var profilesPage = await GetProfilesPageAsync(assignedProfileIds, request, ct);
        if (profilesPage.Metadata.TotalCount == 0)
            return Result.Ok(PaginatedResult<ProfileApprovalListItemDto>.Empty);

        var profileIds = profilesPage.Items.Select(p => p.Id).ToList();

        // 3) Review summaries
        var fullReviewMap = await GetFullReviewSummariesAsync(profileIds, ct);
        var changeReviewMap = await GetChangeRequestSummariesAsync(profileIds, ct);

        // 4) Build DTOs
        var dtoList = BuildDtos(localization, profilesPage.Items, fullReviewMap, changeReviewMap);

        // 5) Apply in-memory filters (search + dropdown filters) + in-memory pagination
        return Result.Ok(new PaginatedResult<ProfileApprovalListItemDto>(
            dtoList,
            profilesPage.Metadata.TotalCount,
            profilesPage.Metadata.CurrentPage,
            profilesPage.Metadata.PageSize
        ));
    }

    // ----------------------------
    // Data access helpers
    // ----------------------------

    private async Task<List<Guid>> GetAssignedProfileIdsAsync(Guid officerId, CancellationToken ct)
    {
        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();

        return await assignmentRepo.DbSet
            .AsNoTracking()
            .Where(a => a.IsActive && a.EmployeeId == officerId)
            .Select(a => a.UserProfileId)
            .Distinct()
            .ToListAsync(ct);
    }

    private async Task<PaginatedResult<UserProfile>> GetProfilesPageAsync(
        List<Guid> assignedProfileIds,
        GetProfileApprovalsQuery request,
        CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();

        return await profileRepo.DbSet
            .AsNoTracking()
            .AsSplitQuery()
            .Include(p => p.User)
            .Include(p => p.CandidateType)
            .Include(p => p.TargetEntity)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.Major)
            .Where(p => assignedProfileIds.Contains(p.Id))
            .Where(p =>
                p.Status == UserProfileStatus.Submitted ||
                p.Status == UserProfileStatus.UnderReview ||
                p.ReviewItems.Any(r => r.Status == ReviewStatus.NotReviewed || r.Status == ReviewStatus.Pending))
            .ToPaginatedListAsync(request, ct);
    }

    private async Task<Dictionary<Guid, FullReviewSummary>> GetFullReviewSummariesAsync(List<Guid> profileIds, CancellationToken ct)
    {
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();

        var summaries = await reviewRepo.DbSet
            .AsNoTracking()
            .Where(r => profileIds.Contains(r.UserProfileId))
            .Where(r => r.ProfileChangeId == null) // phase 1 only
            .Where(r => r.TargetType != ReviewTargetType.Field)
            .GroupBy(r => r.UserProfileId)
            .Select(g => new FullReviewSummary
            {
                UserProfileId = g.Key,
                PendingSections = g.Count(r => r.Status == ReviewStatus.Pending || r.Status == ReviewStatus.NotReviewed),
                FlaggedSections = g.Count(r => r.Status == ReviewStatus.NeedsCorrection),
                ApprovedSections = g.Count(r => r.Status == ReviewStatus.Approved),
                OverallStatus = g.Any(r => r.Status == ReviewStatus.NeedsCorrection)
                    ? ReviewStatus.NeedsCorrection
                    : g.Any(r => r.Status == ReviewStatus.Pending || r.Status == ReviewStatus.NotReviewed)
                        ? ReviewStatus.Pending
                        : ReviewStatus.Approved,
                LastUpdatedAtUtc = g.Max(r => r.UpdatedDate ?? r.CreatedDate)
            })
            .ToListAsync(ct);

        return summaries.ToDictionary(x => x.UserProfileId, x => x);
    }

    private async Task<Dictionary<Guid, ChangeRequestSummary>> GetChangeRequestSummariesAsync(
        List<Guid> profileIds,
        CancellationToken ct)
    {
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();

        var summaries = await reviewRepo.DbSet
            .AsNoTracking()
            .Where(r => profileIds.Contains(r.UserProfileId))
            .Where(r => r.ProfileChangeId != null) // phase 2 only
            .GroupBy(r => r.UserProfileId)
            .Select(g => new ChangeRequestSummary
            {
                UserProfileId = g.Key,
                Outstanding = g.Count(r => r.Status != ReviewStatus.Approved),
                Pending = g.Count(r => r.Status == ReviewStatus.Pending || r.Status == ReviewStatus.NotReviewed),
                Flagged = g.Count(r => r.Status == ReviewStatus.NeedsCorrection),
                Rejected = g.Count(r => r.Status == ReviewStatus.Rejected),
                OverallStatus = g.Any(r => r.Status == ReviewStatus.NeedsCorrection)
                    ? ReviewStatus.NeedsCorrection
                    : g.Any(r => r.Status == ReviewStatus.Rejected)
                        ? ReviewStatus.Rejected
                        : g.Any(r => r.Status == ReviewStatus.Pending || r.Status == ReviewStatus.NotReviewed)
                            ? ReviewStatus.Pending
                            : ReviewStatus.Approved,
                LastUpdatedAtUtc = g.Max(r => r.UpdatedDate ?? r.CreatedDate)
            })
            .ToListAsync(ct);

        return summaries.ToDictionary(x => x.UserProfileId, x => x);
    }

    private static List<ProfileApprovalListItemDto> BuildDtos(ILocalizationService localization,
        List<UserProfile> profiles,
        Dictionary<Guid, FullReviewSummary> fullReviewMap,
        Dictionary<Guid, ChangeRequestSummary> changeSummaryMap)
    {
        var result = new List<ProfileApprovalListItemDto>(profiles.Count);

        foreach (var profile in profiles)
        {
            // Approved profile with outstanding change requests
            if (IsApprovedWithOutstandingChanges(profile, changeSummaryMap, out var change))
            {
                if(change is not null)
                    result.Add(MapPhase2(localization, profile, change));
                continue;
            }

            // Full review queue (Submitted / UnderReview only)
            if (profile.Status is not (UserProfileStatus.Submitted or UserProfileStatus.UnderReview))
                continue;

            fullReviewMap.TryGetValue(profile.Id, out var full);
            result.Add(MapPhase1(localization, profile, full));
        }

        return result;
    }

    private static bool IsApprovedWithOutstandingChanges(
        UserProfile profile,
        Dictionary<Guid, ChangeRequestSummary> changeSummaryMap,
        out ChangeRequestSummary? change)
    {
        change = null;
        if (profile.Status != UserProfileStatus.Approved)
            return false;

        if (!changeSummaryMap.TryGetValue(profile.Id, out change))
            return false;

        return change.Outstanding > 0;
    }

    private static ProfileApprovalListItemDto MapPhase2(ILocalizationService localization, UserProfile profile, ChangeRequestSummary change)
    {
        return new ProfileApprovalListItemDto
        {
            UserProfileId = profile.Id,
            UserId = profile.UserId,

            FullName = localization.GetLocalizedFullName(profile.User),
            CandidateType = localization.GetLocalizedName(profile.CandidateType),
            TargetEntity = localization.GetLocalizedName(profile.TargetEntity),
            Specialization =  localization.GetLocalizedName(profile.Qualifications?.FirstOrDefault()?.Major),
                
            SubmittedAtUtc = change.LastUpdatedAtUtc,
            ProfileStatus = profile.Status,

            PendingCount = change.Pending,
            OverallStatus = change.OverallStatus,
            LastUpdatedAtUtc = change.LastUpdatedAtUtc,

            AllowedOperations = ResolveAllowedOperations(profile.Status)
        };
    }

    private static ProfileApprovalListItemDto MapPhase1(ILocalizationService localization, UserProfile profile, FullReviewSummary? summary)
    {
        var pendingSections = summary?.PendingSections ?? ProfileApprovalFlow.Sections.Length;
        var overallStatus = summary?.OverallStatus ?? ReviewStatus.Pending;

        var lastUpdatedAtUtc =
            summary?.LastUpdatedAtUtc
            ?? profile.UpdatedDate
            ?? profile.CreatedDate;

        return new ProfileApprovalListItemDto
        {
            UserProfileId = profile.Id,
            UserId = profile.UserId,
            FullName = localization.GetLocalizedFullName(profile.User),
            CandidateType = localization.GetLocalizedName(profile.CandidateType),
            TargetEntity = localization.GetLocalizedName(profile.TargetEntity),
            Specialization =  localization.GetLocalizedName(profile.Qualifications?.FirstOrDefault()?.Major),

            SubmittedAtUtc = profile.CreatedDate,
            ProfileStatus = profile.Status,

            PendingCount = pendingSections,
            OverallStatus = overallStatus,
            LastUpdatedAtUtc = lastUpdatedAtUtc,

            AllowedOperations = ResolveAllowedOperations(profile.Status)
        };
    }

    private static PaginatedResult<ProfileApprovalListItemDto> ApplyFiltersAndPagination(
        List<ProfileApprovalListItemDto> list,
        GetProfileApprovalsQuery request)
    {
        var filtered = list
            .WhereIf(!string.IsNullOrWhiteSpace(request.Search), p =>
                p.FullName.Contains(request.Search!, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrWhiteSpace(p.TargetEntity) &&
                 p.TargetEntity.Contains(request.Search!, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(p.CandidateType) &&
                 p.CandidateType.Contains(request.Search!, StringComparison.OrdinalIgnoreCase)))
            .WhereIf(!string.IsNullOrWhiteSpace(request.CandidateType), p =>
                !string.IsNullOrWhiteSpace(p.CandidateType) &&
                p.CandidateType.Contains(request.CandidateType!, StringComparison.OrdinalIgnoreCase))
            .WhereIf(!string.IsNullOrWhiteSpace(request.TargetEntity), p =>
                !string.IsNullOrWhiteSpace(p.TargetEntity) &&
                p.TargetEntity.Contains(request.TargetEntity!, StringComparison.OrdinalIgnoreCase))
            .WhereIf(!string.IsNullOrWhiteSpace(request.Specialization), p =>
                !string.IsNullOrWhiteSpace(p.Specialization) &&
                p.Specialization.Contains(request.Specialization!, StringComparison.OrdinalIgnoreCase))
            .WhereIf(request.Status is not null, p => p.OverallStatus == request.Status)
            .ToPaginatedList(request);

        return filtered;
    }

    private static IReadOnlyList<string> ResolveAllowedOperations(UserProfileStatus status)
    {
        var ops = new List<string> { ProfileApprovalOperations.View };

        // Phase 1: reviewer can review/finalize
        if (status is UserProfileStatus.Submitted or UserProfileStatus.UnderReview)
        {
            ops.Add(ProfileApprovalOperations.Review);
            ops.Add(ProfileApprovalOperations.Finalize);
        }

        // Phase 2: view only (optional marker)
        if (status == UserProfileStatus.Approved)
            ops.Add(ProfileApprovalOperations.ViewOnly);

        return ops;
    }

    // ----------------------------
    // Local summary models (readability)
    // ----------------------------
}
