using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.DTOs;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Handlers.Queries;

public sealed class GetProfileApprovalsHandler(IUnitOfWork uow)
    : IRequestHandler<GetProfileApprovalsQuery, Result<IReadOnlyList<ProfileApprovalListItemDto>>>
{
    public async Task<Result<IReadOnlyList<ProfileApprovalListItemDto>>> Handle(
        GetProfileApprovalsQuery request,
        CancellationToken ct)
    {
        if (request.OfficerId == Guid.Empty)
            return Result.Fail<IReadOnlyList<ProfileApprovalListItemDto>>(ErrorsCodes.InvalidUserIdentifier);

        // 1) Get assigned profiles (active assignments only)
        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();

        var assignedProfileIds = await assignmentRepo.DbSet
            .AsNoTracking()
            .Where(a => a.IsActive && a.EmployeeId == request.OfficerId)
            .Select(a => a.UserProfileId)
            .Distinct()
            .ToListAsync(ct);

        if (assignedProfileIds.Count == 0)
            return Result.Ok<IReadOnlyList<ProfileApprovalListItemDto>>([]);

        // 2) Load assigned profiles for:
        // - Full review (phase 1): Submitted / UnderReview
        // - Change requests (phase 2): Approved + pending change review items
        var profileRepo = uow.GetEntityRepository<UserProfile>();

        var profiles = await profileRepo.DbSet
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
                p.Status == UserProfileStatus.Approved)
            .ToListAsync(ct);

        if (profiles.Count == 0)
            return Result.Ok<IReadOnlyList<ProfileApprovalListItemDto>>([]);

        var profileIds = profiles.Select(p => p.Id).ToList();

        var reviewRepo = uow.GetEntityRepository<ReviewItem>();

        // 3-a) Full Review summary: Section-only, ProfileChangeId == null (phase 1)
        var fullReviewSummaries = await reviewRepo.DbSet
            .AsNoTracking()
            .Where(r => profileIds.Contains(r.UserProfileId))
            .Where(r => r.TargetType == ReviewTargetType.Section)
            .Where(r => r.ProfileChangeId == null) // phase 1 only
            .GroupBy(r => r.UserProfileId)
            .Select(g => new
            {
                UserProfileId = g.Key,

                PendingSections = g.Count(r =>
                    r.Status == ReviewStatus.Pending ||
                    r.Status == ReviewStatus.NotReviewed),

                FlaggedSections = g.Count(r => r.Status == ReviewStatus.NeedsCorrection),

                ApprovedSections = g.Count(r => r.Status == ReviewStatus.Approved),

                OverallStatus =
                    g.Any(r => r.Status == ReviewStatus.NeedsCorrection)
                        ? ReviewStatus.NeedsCorrection
                        : g.Any(r => r.Status == ReviewStatus.Pending || r.Status == ReviewStatus.NotReviewed)
                            ? ReviewStatus.Pending
                            : ReviewStatus.Approved,

                LastUpdatedAtUtc = g.Max(r => r.UpdatedDate ?? r.CreatedDate)
            })
            .ToListAsync(ct);

        var fullSummaryMap = fullReviewSummaries.ToDictionary(x => x.UserProfileId, x => x);

        // 3-b) Change Requests summary: ProfileChangeId != null (phase 2)
        var changeSummaries = await reviewRepo.DbSet
            .AsNoTracking()
            .Where(r => profileIds.Contains(r.UserProfileId))
            .Where(r => r.ProfileChangeId != null) // phase 2 only
            .GroupBy(r => r.UserProfileId)
            .Select(g => new
            {
                UserProfileId = g.Key,
                Outstanding = g.Count(r => r.Status != ReviewStatus.Approved),
                Pending = g.Count(r => r.Status == ReviewStatus.Pending || r.Status == ReviewStatus.NotReviewed),
                Flagged = g.Count(r => r.Status == ReviewStatus.NeedsCorrection),
                Rejected = g.Count(r => r.Status == ReviewStatus.Rejected),
                OverallStatus =
                    g.Any(r => r.Status == ReviewStatus.NeedsCorrection)
                        ? ReviewStatus.NeedsCorrection
                        : g.Any(r => r.Status == ReviewStatus.Rejected)
                            ? ReviewStatus.Rejected
                            : g.Any(r => r.Status == ReviewStatus.Pending || r.Status == ReviewStatus.NotReviewed)
                                ? ReviewStatus.Pending
                                : ReviewStatus.Approved,
                LastUpdatedAtUtc = g.Max(r => r.UpdatedDate ?? r.CreatedDate)
            })
            .ToListAsync(ct);

        var changeSummaryMap = changeSummaries.ToDictionary(x => x.UserProfileId, x => x);

        // 4) Build list DTOs (phase 1 + phase 2)
        var list = profiles
            .Select(profile =>
            {
                // Phase 2: Approved profile with outstanding change requests
                if (profile.Status == UserProfileStatus.Approved &&
                    changeSummaryMap.TryGetValue(profile.Id, out var change) &&
                    change.Outstanding > 0)
                {
                    return new ProfileApprovalListItemDto
                    {
                        UserProfileId = profile.Id,
                        UserId = profile.UserId,
                        FullName = profile.User?.FullNameEn ?? profile.User?.FullNameAr ?? string.Empty,

                        CandidateType = profile.CandidateType?.NameEn ?? profile.CandidateType?.NameAr,
                        TargetEntity = profile.TargetEntity?.NameEn ?? profile.TargetEntity?.NameAr,

                        Specialization = profile.Qualifications?.FirstOrDefault()?.Major?.NameEn
                                         ?? profile.Qualifications?.FirstOrDefault()?.Major?.NameAr,

                        SubmittedAtUtc =  change.LastUpdatedAtUtc,
                        ProfileStatus = profile.Status,

                        PendingCount = change.Pending,
                        OverallStatus = change.OverallStatus,
                        LastUpdatedAtUtc = change.LastUpdatedAtUtc,

                        AllowedOperations = ResolveAllowedOperations(profile.Status)
                    };
                }

                // Phase 1: Full review queue (Submitted / UnderReview)
                if (profile.Status is not (UserProfileStatus.Submitted or UserProfileStatus.UnderReview))
                    return null;

                fullSummaryMap.TryGetValue(profile.Id, out var summary);

                var pendingSections = summary?.PendingSections
                                      ?? ProfileApprovalFlow.Sections.Length; // if no review items found, treat as all pending

                var overallStatus = summary?.OverallStatus ?? ReviewStatus.Pending;

                var submittedAtUtc = profile.CreatedDate;

                var lastUpdated =
                    summary?.LastUpdatedAtUtc
                    ?? profile.UpdatedDate
                    ?? profile.CreatedDate;

                return new ProfileApprovalListItemDto
                {
                    UserProfileId = profile.Id,
                    UserId = profile.UserId,
                    FullName = profile.User?.FullNameEn ?? profile.User?.FullNameAr ?? string.Empty,

                    CandidateType = profile.CandidateType?.NameEn ?? profile.CandidateType?.NameAr,
                    TargetEntity = profile.TargetEntity?.NameEn ?? profile.TargetEntity?.NameAr,

                    Specialization = profile.Qualifications?.FirstOrDefault()?.Major?.NameEn
                                     ?? profile.Qualifications?.FirstOrDefault()?.Major?.NameAr,

                    SubmittedAtUtc = submittedAtUtc,
                    ProfileStatus = profile.Status,

                    // In phase 1, we want pending count to represent pending sections only
                    PendingCount = pendingSections,

                    OverallStatus = overallStatus,
                    LastUpdatedAtUtc = lastUpdated,

                    AllowedOperations = ResolveAllowedOperations(profile.Status)
                };
            })
            .Where(x => x is not null)
            .Select(x => x!)
            .ToList();

        // 5) Filters (search + dropdown filters)
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
            .AsQueryable();

        // 6) Sorting
        var sortDirection = string.Equals(request.SortDirection, "asc", StringComparison.OrdinalIgnoreCase)
            ? "asc"
            : "desc";

        filtered = request.Sort?.ToLowerInvariant() switch
        {
            "name" => sortDirection == "asc"
                ? filtered.OrderBy(p => p.FullName)
                : filtered.OrderByDescending(p => p.FullName),

            "status" => sortDirection == "asc"
                ? filtered.OrderBy(p => p.OverallStatus)
                : filtered.OrderByDescending(p => p.OverallStatus),

            "entity" => sortDirection == "asc"
                ? filtered.OrderBy(p => p.TargetEntity)
                : filtered.OrderByDescending(p => p.TargetEntity),

            _ => sortDirection == "asc"
                ? filtered.OrderBy(p => p.LastUpdatedAtUtc ?? p.SubmittedAtUtc)
                : filtered.OrderByDescending(p => p.LastUpdatedAtUtc ?? p.SubmittedAtUtc)
        };

        return Result.Ok<IReadOnlyList<ProfileApprovalListItemDto>>(filtered.ToList());
    }

    private static IReadOnlyList<string> ResolveAllowedOperations(UserProfileStatus status)
    {
        // Phase 1:
        // - Submitted/UnderReview => reviewer can view + decide sections + finalize
        // - Approved => view only (optional if you include Approved in list)
        var ops = new List<string> { "view" };

        if (status is UserProfileStatus.Submitted or UserProfileStatus.UnderReview)
        {
            ops.Add("review");
            ops.Add("finalize");
        }

        if (status == UserProfileStatus.Approved)
            ops.Add("view-only");

        return ops;
    }
}
