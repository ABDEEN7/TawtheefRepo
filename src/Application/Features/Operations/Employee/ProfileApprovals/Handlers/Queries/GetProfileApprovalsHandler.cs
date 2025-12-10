using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.DTOs;
using Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.Queries;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.Handlers.Queries;

public class GetProfileApprovalsHandler(IUnitOfWork uow)
    : IRequestHandler<GetProfileApprovalsQuery, Result<IReadOnlyList<ProfileApprovalListItemDto>>>
{
    public async Task<Result<IReadOnlyList<ProfileApprovalListItemDto>>> Handle(GetProfileApprovalsQuery request, CancellationToken ct)
    {
        var submissionRepo = uow.GetEntityRepository<ProfileSubmission>();
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();

        var latestSubmissions = await submissionRepo.DbSet
            .GroupBy(s => s.UserProfileId)
            .Select(g => g.OrderByDescending(s => s.Version).First())
            .ToListAsync(ct);

        if (latestSubmissions.Count == 0)
            return Result.Ok<IReadOnlyList<ProfileApprovalListItemDto>>([]);

        var profileIds = latestSubmissions.Select(s => s.UserProfileId).ToList();

        var profiles = await profileRepo.DbSet
            .Include(p => p.User)
            .Include(p => p.CandidateType)
            .Include(p => p.TargetEntity)
            .Where(p => profileIds.Contains(p.Id))
            .ToListAsync(ct);

        var reviewSummaries = await reviewRepo.DbSet
            .Where(r => profileIds.Contains(r.UserProfileId))
            .GroupBy(r => r.UserProfileId)
            .Select(g => new
            {
                g.Key,
                Pending = g.Count(r => (r.Status == ReviewStatus.Pending || r.Status == ReviewStatus.ChangesRequested) || r.IsOutdated),
                OverallStatus =
                    g.Any(r => r.Status == ReviewStatus.Rejected)
                        ? ReviewStatus.Rejected
                        : g.Any(r => r.Status == ReviewStatus.Pending || r.IsOutdated)
                            ? ReviewStatus.Pending
                            : g.Any(r => r.Status == ReviewStatus.ChangesRequested)
                                ? ReviewStatus.ChangesRequested
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

                return new ProfileApprovalListItemDto
                {
                    UserProfileId = submission.UserProfileId,
                    UserId = profile?.UserId ?? Guid.Empty,
                    FullName = profile?.User?.FullNameEn ?? profile?.User?.FullNameAr ?? "",
                    CandidateType = profile?.CandidateType?.NameAr ?? profile?.CandidateType?.NameEn,
                    TargetEntity = profile?.TargetEntity?.NameAr ?? profile?.TargetEntity?.NameEn,
                    SubmittedAtUtc = submission.SubmittedAtUtc,
                    PendingCount = pending,
                    OverallStatus = overallStatus,
                    LastUpdatedAtUtc = lastUpdatedAtUtc.DateTime
                };
            })
            .OrderByDescending(p => p.LastUpdatedAtUtc ?? p.SubmittedAtUtc)
            .ToList();

        return Result.Ok<IReadOnlyList<ProfileApprovalListItemDto>>(list);
    }
}
