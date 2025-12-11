using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.DTOs;
using Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.Handlers.Queries;

public class GetProfileApprovalsHandler(IUnitOfWork uow)
    : IRequestHandler<GetProfileApprovalsQuery, Result<IReadOnlyList<ProfileApprovalListItemDto>>>
{
    public async Task<Result<IReadOnlyList<ProfileApprovalListItemDto>>> Handle(GetProfileApprovalsQuery request, CancellationToken ct)
    {
        if (request.OfficerId == Guid.Empty)
            return Result.Fail<IReadOnlyList<ProfileApprovalListItemDto>>(ErrorsCodes.InvalidUserIdentifier);

        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();
        var submissionRepo = uow.GetEntityRepository<ProfileSubmission>();

        var assignedProfileIds = await assignmentRepo.DbSet
            .Where(a => a.IsActive && a.EmployeeId == request.OfficerId)
            .Select(a => a.UserProfileId)
            .ToListAsync(ct);

        if (assignedProfileIds.Count == 0)
            return Result.Ok<IReadOnlyList<ProfileApprovalListItemDto>>([]);

        var latestSubmissions = await submissionRepo.DbSet
            .Where(s => assignedProfileIds.Contains(s.UserProfileId))
            .GroupBy(s => s.UserProfileId)
            .Select(g => g.OrderByDescending(s => s.Version).First())
            .ToListAsync(ct);

        if (latestSubmissions.Count == 0)
            return Result.Ok<IReadOnlyList<ProfileApprovalListItemDto>>([]);

        var profileIds = latestSubmissions.Select(s => s.UserProfileId).ToList();

        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var profiles = await profileRepo.DbSet
            .Include(p => p.User)
            .Include(p => p.CandidateType)
            .Include(p => p.TargetEntity)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.Major)
            .Where(p => profileIds.Contains(p.Id))
            .ToListAsync(ct);

        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        var reviewSummaries = await reviewRepo.DbSet
            .Where(r => profileIds.Contains(r.UserProfileId))
            .GroupBy(r => r.UserProfileId)
            .Select(g => new
            {
                g.Key,
                Pending = g.Count(r => (r.Status == ReviewStatus.Pending || r.Status == ReviewStatus.NeedsCorrection) || r.IsOutdated),
                OverallStatus =
                    g.Any(r => r.Status == ReviewStatus.Rejected)
                        ? ReviewStatus.Rejected
                        : g.Any(r => r.Status == ReviewStatus.Pending || r.IsOutdated)
                            ? ReviewStatus.Pending
                            : g.Any(r => r.Status == ReviewStatus.NeedsCorrection)
                                ? ReviewStatus.NeedsCorrection
                                : ReviewStatus.Approved,
                LastUpdatedAtUtc = g.Max(r => r.UpdatedDate ?? r.CreatedDate)
            })
            .ToListAsync(ct);

        var list = latestSubmissions
            .Select(submission =>
            {
                var profile = profiles.FirstOrDefault(p => p.Id == submission.UserProfileId);
                var summary = reviewSummaries.FirstOrDefault(c => c.Key == submission.UserProfileId);
                var pending = summary?.Pending ?? 0;
                var overallStatus = summary?.OverallStatus ?? ReviewStatus.Pending;
                var lastUpdatedAtUtc = summary?.LastUpdatedAtUtc ?? submission.SubmittedAtUtc;
                var profileStatus = profile?.Status ?? UserProfileStatus.Submitted;

                return new ProfileApprovalListItemDto
                {
                    UserProfileId = submission.UserProfileId,
                    UserId = profile?.UserId ?? Guid.Empty,
                    FullName = profile?.User?.FullNameEn ?? profile?.User?.FullNameAr ?? "",
                    CandidateType = profile?.CandidateType?.NameAr ?? profile?.CandidateType?.NameEn,
                    TargetEntity = profile?.TargetEntity?.NameAr ?? profile?.TargetEntity?.NameEn,
                    Specialization = profile?.Qualifications?.FirstOrDefault()?.Major?.NameAr ?? profile?.Qualifications?.FirstOrDefault()?.Major?.NameEn,
                    SubmittedAtUtc = submission.SubmittedAtUtc,
                    ProfileStatus = profileStatus,
                    PendingCount = pending,
                    OverallStatus = overallStatus,
                    LastUpdatedAtUtc = lastUpdatedAtUtc.DateTime,
                    AllowedOperations = ResolveAllowedOperations(profileStatus)
                };
            })
            .ToList();

        var filtered = list.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
            filtered = filtered.Where(p =>
                p.FullName.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrWhiteSpace(p.TargetEntity) && p.TargetEntity.Contains(request.Search, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(p.CandidateType) && p.CandidateType.Contains(request.Search, StringComparison.OrdinalIgnoreCase)));

        if (!string.IsNullOrWhiteSpace(request.CandidateType))
            filtered = filtered.Where(p => !string.IsNullOrWhiteSpace(p.CandidateType) && p.CandidateType.Contains(request.CandidateType, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.TargetEntity))
            filtered = filtered.Where(p => !string.IsNullOrWhiteSpace(p.TargetEntity) && p.TargetEntity.Contains(request.TargetEntity, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.Specialization))
            filtered = filtered.Where(p => !string.IsNullOrWhiteSpace(p.Specialization) && p.Specialization.Contains(request.Specialization, StringComparison.OrdinalIgnoreCase));

        if (request.Status is not null)
            filtered = filtered.Where(p => p.OverallStatus == request.Status);

        var sortDirection = string.Equals(request.SortDirection, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";

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
        var ops = new List<string> { "view" };

        if (status is UserProfileStatus.Submitted or UserProfileStatus.UnderReview or UserProfileStatus.RequiresUpdate)
        {
            ops.Add("review");
            ops.Add("finalize");
        }

        if (status == UserProfileStatus.Approved)
            ops.Add("view-only");

        return ops;
    }
}
