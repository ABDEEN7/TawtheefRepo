using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.ProfileApproval;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Handlers.Queries;

public sealed class GetProfileApprovalsHandler(IUnitOfWork uow, ILocalizationService localization)
    : IRequestHandler<GetProfileApprovalsQuery, Result<PaginatedResult<ProfileApprovalListItemDto>>>
{
    public async Task<Result<PaginatedResult<ProfileApprovalListItemDto>>> Handle(GetProfileApprovalsQuery request, CancellationToken ct)
    {
        // Guard clauses
        if (request.OfficerId == Guid.Empty)
            return Result.Fail<PaginatedResult<ProfileApprovalListItemDto>>(ErrorsCodes.InvalidUserIdentifier);

        // 1) Build base DB query with minimal filters
        var profileRepo = uow.GetEntityRepository<UserProfile>();

        var baseQuery = profileRepo.DbSet
            .AsNoTracking()
            .Where(p => p.ProfileAssignments.Any(a => a.IsActive && a.EmployeeId == request.OfficerId))
            .Where(p =>
                p.Status == UserProfileStatus.Submitted ||
                p.Status == UserProfileStatus.UnderReview ||
                p.ReviewItems.Any(r => r.Status == ReviewStatus.NotReviewed || r.Status == ReviewStatus.Pending));

        // 2) Apply DB-level filters
        baseQuery = ApplyDatabaseFilters(baseQuery, request);

        // 3) Project and Paginate in ONE query
        // This avoids 3 sequential roundtrips and loading 300+ columns from UserProfile
        var dataPage = await baseQuery
            .Select(p => new ProfileRowData
            {
                Id = p.Id,
                UserId = p.UserId,
                User = p.User,
                CandidateType = p.CandidateType,
                TargetEntity = p.TargetEntity,
                Status = p.Status,
                CreatedDate = p.CreatedDate,
                UpdatedDate = p.UpdatedDate,

                // Phase 1 summary (Full Review)
                HasP1Pending = p.ReviewItems.Any(r => r.ProfileChangeId == null && r.TargetType != ReviewTargetType.Field && (r.Status == ReviewStatus.Pending || r.Status == ReviewStatus.NotReviewed || r.Status == ReviewStatus.Solved)),
                HasP1Flagged = p.ReviewItems.Any(r => r.ProfileChangeId == null && r.TargetType != ReviewTargetType.Field && r.Status == ReviewStatus.NeedsCorrection),
                P1MaxDate = p.ReviewItems.Where(r => r.ProfileChangeId == null && r.TargetType != ReviewTargetType.Field).Max(r => (DateTimeOffset?)(r.UpdatedDate ?? r.CreatedDate)),

                // Phase 2 summary (Change Requests)
                HasP2Outstanding = p.ReviewItems.Any(r => r.ProfileChangeId != null && r.Status != ReviewStatus.Approved),
                HasP2Pending = p.ReviewItems.Any(r => r.ProfileChangeId != null && (r.Status == ReviewStatus.Pending || r.Status == ReviewStatus.NotReviewed || r.Status == ReviewStatus.Solved)),
                HasP2Flagged = p.ReviewItems.Any(r => r.ProfileChangeId != null && r.Status == ReviewStatus.NeedsCorrection),
                HasP2Rejected = p.ReviewItems.Any(r => r.ProfileChangeId != null && r.Status == ReviewStatus.Rejected)
            })
            .ToPaginatedListAsync(request, ct);

        if (dataPage.Metadata.TotalCount == 0)
            return Result.Ok(PaginatedResult<ProfileApprovalListItemDto>.Empty);

        // 4) Build final DTOs in memory
        var dtoList = BuildDtos(localization, dataPage.Items);

        return Result.Ok(new PaginatedResult<ProfileApprovalListItemDto>(
            dtoList,
            dataPage.Metadata.TotalCount,
            dataPage.Metadata.CurrentPage,
            dataPage.Metadata.PageSize
        ));
    }

    private static IQueryable<UserProfile> ApplyDatabaseFilters(IQueryable<UserProfile> query, GetProfileApprovalsQuery request)
    {
        if (request.TargetEntityId.HasValue)
            query = query.Where(p => p.TargetEntityId == request.TargetEntityId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(p =>
                p.User!.FullNameAr.Contains(search) ||
                p.User.FullNameEn.Contains(search) ||
                (p.TargetEntity != null && (p.TargetEntity.NameAr.Contains(search) || p.TargetEntity.NameEn.Contains(search))) ||
                (p.CandidateType != null && (p.CandidateType.NameAr.Contains(search) || p.CandidateType.NameEn.Contains(search))));
        }
        
        if (!string.IsNullOrWhiteSpace(request.CandidateType))
        {
            var ctValue = request.CandidateType.Trim();
            query = query.Where(p => p.CandidateType != null &&  (p.CandidateType.NameAr.Contains(ctValue) || p.CandidateType.NameEn.Contains(ctValue)));
        }


        return query;
    }

    private static List<ProfileApprovalListItemDto> BuildDtos(ILocalizationService localization, List<ProfileRowData> data)
    {
        var result = new List<ProfileApprovalListItemDto>(data.Count);

        foreach (var row in data)
        {
            // Phase 2 logic (Approved with outstanding changes)
            if (row.Status == UserProfileStatus.Approved && row.HasP2Outstanding)
            {
                result.Add(MapPhase2(localization, row));
                continue;
            }

            // Phase 1 logic (Submitted / UnderReview)
            if (row.Status is UserProfileStatus.Submitted or UserProfileStatus.UnderReview)
            {
                result.Add(MapPhase1(localization, row));
            }
        }

        return result;
    }

    private static ProfileApprovalListItemDto MapPhase2(ILocalizationService localization, ProfileRowData row)
    {
        var overallStatus = row.HasP2Flagged ? ReviewStatus.NeedsCorrection
                          : row.HasP2Rejected ? ReviewStatus.Rejected
                          : row.HasP2Pending ? ReviewStatus.Pending
                          : ReviewStatus.Approved;

        return new ProfileApprovalListItemDto
        {
            UserProfileId = row.Id,
            UserId = row.UserId,
            FullName = localization.GetLocalizedFullName(row.User),
            CandidateType = localization.GetLocalizedName(row.CandidateType),
            TargetEntity = localization.GetLocalizedName(row.TargetEntity),
            SubmittedAtUtc = row.UpdatedDate ?? row.CreatedDate,
            ProfileStatus = row.Status,
            OverallStatus = overallStatus,
            LastUpdatedAtUtc = row.UpdatedDate ?? row.CreatedDate,
            AllowedOperations = ResolveAllowedOperations(row.Status)
        };
    }

    private static ProfileApprovalListItemDto MapPhase1(ILocalizationService localization, ProfileRowData row)
    {
        // Fallback for brand new profiles without review tracking items yet
        var hasAnyReview = row.HasP1Pending || row.HasP1Flagged;
        
        var overallStatus = row.HasP1Flagged ? ReviewStatus.NeedsCorrection
                          : (row.HasP1Pending || !hasAnyReview) ? ReviewStatus.Pending
                          : ReviewStatus.Approved;

        return new ProfileApprovalListItemDto
        {
            UserProfileId = row.Id,
            UserId = row.UserId,
            FullName = localization.GetLocalizedFullName(row.User),
            CandidateType = localization.GetLocalizedName(row.CandidateType),
            TargetEntity = localization.GetLocalizedName(row.TargetEntity),
            SubmittedAtUtc = row.CreatedDate,
            ProfileStatus = row.Status,
            OverallStatus = overallStatus,
            LastUpdatedAtUtc = row.P1MaxDate ?? row.UpdatedDate ?? row.CreatedDate,
            AllowedOperations = ResolveAllowedOperations(row.Status)
        };
    }

    private record ProfileRowData
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public ApplicantUser? User { get; init; }
        public CandidateType? CandidateType { get; init; }
        public TargetEntity? TargetEntity { get; init; }
        public UserProfileStatus Status { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
        public DateTimeOffset? UpdatedDate { get; init; }
        
        public bool HasP1Pending { get; init; }
        public bool HasP1Flagged { get; init; }
        public DateTimeOffset? P1MaxDate { get; init; }

        public bool HasP2Outstanding { get; init; }
        public bool HasP2Pending { get; init; }
        public bool HasP2Flagged { get; init; }
        public bool HasP2Rejected { get; init; }
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

