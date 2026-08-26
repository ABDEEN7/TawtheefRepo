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
using Tawtheef.Domain.Entities.Kawader;
using Tawtheef.Domain.Entities.MinisterOffice;
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
        var ministerOfficeCandidates = uow.GetEntityRepository<MinisterOfficeCandidate>().DbSet.AsNoTracking();
        var kawaaderCandidates = uow.GetEntityRepository<KawaderQid>().DbSet.AsNoTracking();

        var baseQuery = profileRepo.DbSet
            .AsNoTracking()
            .Where(p => p.ProfileAssignments.Any(a => a.IsActive && a.EmployeeId == request.OfficerId))
            .Where(p => p.Status == UserProfileStatus.Submitted || p.Status == UserProfileStatus.UnderReview);

        // 2) Apply DB-level filters
        baseQuery = ApplyDatabaseFilters(
            baseQuery,
            request,
            ministerOfficeCandidates,
            kawaaderCandidates);

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
                TargetEntityId = p.TargetEntityId,
                Status = p.Status,
                CreatedDate = p.CreatedDate,
                UpdatedDate = p.UpdatedDate,
                IsMinisterOfficeCandidate = p.NationalNumber != null && ministerOfficeCandidates.Any(candidate =>
                    !candidate.IsDeleted && candidate.IsFollowUpActive && candidate.Qid == p.NationalNumber),
                IsKawaaderCandidate = p.NationalNumber != null && kawaaderCandidates.Any(candidate =>
                    candidate.Qid == p.NationalNumber),

                // Phase 1 summary (Full Review)
                HasP1AnyReview = p.ReviewItems.Any(r => r.ProfileChangeId == null && r.TargetType != ReviewTargetType.Field),
                HasP1Pending = p.ReviewItems.Any(r => r.ProfileChangeId == null && r.TargetType != ReviewTargetType.Field && (r.Status == ReviewStatus.Pending || r.Status == ReviewStatus.NotReviewed || r.Status == ReviewStatus.Solved)),
                HasP1Flagged = p.ReviewItems.Any(r => r.ProfileChangeId == null && r.TargetType != ReviewTargetType.Field && r.Status == ReviewStatus.NeedsCorrection),
                HasP1Rejected = p.ReviewItems.Any(r => r.ProfileChangeId == null && r.TargetType != ReviewTargetType.Field && r.Status == ReviewStatus.Rejected),
                P1MaxDate = p.ReviewItems.Where(r => r.ProfileChangeId == null && r.TargetType != ReviewTargetType.Field).Max(r => (DateTimeOffset?)(r.UpdatedDate ?? r.CreatedDate))
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

    private static IQueryable<UserProfile> ApplyDatabaseFilters(
        IQueryable<UserProfile> query,
        GetProfileApprovalsQuery request,
        IQueryable<MinisterOfficeCandidate> ministerOfficeCandidates,
        IQueryable<KawaderQid> kawaaderCandidates)
    {
        var targetEntityIds = request.TargetEntityIds?.Distinct().ToArray() ?? [];
        if (targetEntityIds.Length > 0)
            query = query.Where(p => p.TargetEntityId.HasValue && targetEntityIds.Contains(p.TargetEntityId.Value));

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(p =>
                p.User!.FullNameAr.Contains(search) ||
                p.User.FullNameEn.Contains(search) ||
                (p.NationalNumber != null && p.NationalNumber.Contains(search)) ||
                (p.User.Email != null && p.User.Email.Contains(search)));
        }
        
        var candidateTypeIds = request.CandidateTypeIds?.Distinct().ToArray() ?? [];
        if (candidateTypeIds.Length > 0)
            query = query.Where(p => p.CandidateTypeId.HasValue && candidateTypeIds.Contains(p.CandidateTypeId.Value));

        var candidateSources = request.CandidateSources?.Distinct().ToArray() ?? [];
        if (candidateSources.Length > 0)
        {
            var includeMinisterOffice = candidateSources.Contains(ProfileApprovalCandidateSource.MinisterOffice);
            var includeKawaader = candidateSources.Contains(ProfileApprovalCandidateSource.Kawaader);

            query = query.Where(profile => profile.NationalNumber != null &&
                (includeMinisterOffice && ministerOfficeCandidates.Any(candidate =>
                    !candidate.IsDeleted && candidate.IsFollowUpActive && candidate.Qid == profile.NationalNumber) ||
                 includeKawaader && kawaaderCandidates.Any(candidate =>
                    candidate.Qid == profile.NationalNumber)));
        }

        query = ApplyReviewStatusFilter(query, request.Statuses);


        return query;
    }

    private static IQueryable<UserProfile> ApplyReviewStatusFilter(
        IQueryable<UserProfile> query,
        IReadOnlyCollection<ReviewStatus>? requestedStatuses)
    {
        var statuses = requestedStatuses?.Distinct().ToArray() ?? [];
        if (statuses.Length == 0) return query;

        var includePending = statuses.Contains(ReviewStatus.Pending);
        var includeApproved = statuses.Contains(ReviewStatus.Approved);
        var includeRejected = statuses.Contains(ReviewStatus.Rejected);
        var includeNeedsCorrection = statuses.Contains(ReviewStatus.NeedsCorrection);

        return query.Where(profile =>
            includeNeedsCorrection && profile.ReviewItems.Any(review =>
                review.ProfileChangeId == null && review.TargetType != ReviewTargetType.Field && review.Status == ReviewStatus.NeedsCorrection)
            || includeRejected
               && !profile.ReviewItems.Any(review => review.ProfileChangeId == null &&
                   review.TargetType != ReviewTargetType.Field && review.Status == ReviewStatus.NeedsCorrection)
               && profile.ReviewItems.Any(review => review.ProfileChangeId == null &&
                   review.TargetType != ReviewTargetType.Field && review.Status == ReviewStatus.Rejected)
            || includePending
               && !profile.ReviewItems.Any(review => review.ProfileChangeId == null &&
                   review.TargetType != ReviewTargetType.Field &&
                   (review.Status == ReviewStatus.NeedsCorrection || review.Status == ReviewStatus.Rejected))
               && (profile.ReviewItems.Any(review => review.ProfileChangeId == null &&
                       review.TargetType != ReviewTargetType.Field &&
                       (review.Status == ReviewStatus.Pending || review.Status == ReviewStatus.NotReviewed || review.Status == ReviewStatus.Solved))
                   || !profile.ReviewItems.Any(review => review.ProfileChangeId == null && review.TargetType != ReviewTargetType.Field))
            || includeApproved
               && profile.ReviewItems.Any(review => review.ProfileChangeId == null && review.TargetType != ReviewTargetType.Field)
               && !profile.ReviewItems.Any(review => review.ProfileChangeId == null &&
                   review.TargetType != ReviewTargetType.Field &&
                   (review.Status == ReviewStatus.NeedsCorrection || review.Status == ReviewStatus.Rejected ||
                    review.Status == ReviewStatus.Pending || review.Status == ReviewStatus.NotReviewed || review.Status == ReviewStatus.Solved)));
    }

    private static List<ProfileApprovalListItemDto> BuildDtos(ILocalizationService localization, List<ProfileRowData> data)
    {
        return data.Select(row => MapPhase1(localization, row)).ToList();
    }

    private static ProfileApprovalListItemDto MapPhase1(ILocalizationService localization, ProfileRowData row)
    {
        // Fallback for brand new profiles without review tracking items yet
        var hasAnyReview = row.HasP1AnyReview;
        
        var overallStatus = row.HasP1Flagged ? ReviewStatus.NeedsCorrection
                          : row.HasP1Rejected ? ReviewStatus.Rejected
                          : (row.HasP1Pending || !hasAnyReview) ? ReviewStatus.Pending
                          : ReviewStatus.Approved;

        return new ProfileApprovalListItemDto
        {
            UserProfileId = row.Id,
            UserId = row.UserId,
            FullName = localization.GetLocalizedFullName(row.User),
            CandidateType = localization.GetLocalizedName(row.CandidateType),
            TargetEntity = localization.GetLocalizedName(row.TargetEntity),
            IsMinisterOfficeCandidate = row.IsMinisterOfficeCandidate,
            IsKawaaderCandidate = row.IsKawaaderCandidate,
            SubmittedAtUtc = row.CreatedDate,
            ProfileStatus = row.Status,
            OverallStatus = overallStatus,
            LastUpdatedAtUtc = row.P1MaxDate ?? row.UpdatedDate ?? row.CreatedDate,
            AllowedOperations = ResolveAllowedOperations()
        };
    }

    private record ProfileRowData
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public ApplicantUser? User { get; init; }
        public CandidateType? CandidateType { get; init; }
        public TargetEntity? TargetEntity { get; init; }
        public Guid? TargetEntityId { get; init; }
        public UserProfileStatus Status { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
        public DateTimeOffset? UpdatedDate { get; init; }
        public bool IsMinisterOfficeCandidate { get; init; }
        public bool IsKawaaderCandidate { get; init; }
        
        public bool HasP1AnyReview { get; init; }
        public bool HasP1Pending { get; init; }
        public bool HasP1Flagged { get; init; }
        public bool HasP1Rejected { get; init; }
        public DateTimeOffset? P1MaxDate { get; init; }
    }

    private static IReadOnlyList<string> ResolveAllowedOperations()
    {
        return
        [
            ProfileApprovalOperations.View,
            ProfileApprovalOperations.Review,
            ProfileApprovalOperations.Finalize
        ];
    }

    // ----------------------------
    // Local summary models (readability)
    // ----------------------------
}

