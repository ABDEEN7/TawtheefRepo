using FluentResults;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Mappers;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.DTOs;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Queries;
using Tawtheef.Application.Features.Recruitment.Profile;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Handlers.Queries;

public class GetProfileApprovalDetailHandler(IUnitOfWork uow, IMapper mapper, IMediaUrlResolver media, ILocalizationService localization)
    : IRequestHandler<GetProfileApprovalDetailQuery, Result<GetProfileApprovalDetailDto>>
{
    public async Task<Result<GetProfileApprovalDetailDto>> Handle(GetProfileApprovalDetailQuery request,
        CancellationToken ct)
    {
        
        var profile = await UserProfileLoader.GetFullProfileByProfileId(uow, request.UserProfileId, ct: ct);
        if (profile is null)
            return Result.Fail<GetProfileApprovalDetailDto>(ErrorsCodes.UserProfileNotFound);

        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();
        var auditRepo = uow.GetEntityRepository<AuditTrailEntry>();

        var isAssigned = await assignmentRepo.DbSet
            .AsNoTracking()
            .AnyAsync(a => a.UserProfileId == profile.Id && a.EmployeeId == request.OfficerId && a.IsActive, ct);

        if (!isAssigned)
            return Result.Fail<GetProfileApprovalDetailDto>(ErrorsCodes.UnauthorizedAction);

        // ===== Reviews =====
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        var reviewItems = await reviewRepo.DbSet
            .AsNoTracking()
            .Where(r => r.UserProfileId == profile.Id && r.ProfileChangeId == null)
            .ToListAsync(ct);
        var sectionItems = reviewItems.Where(x => x.TargetType == ReviewTargetType.Section);
        var detailItems = reviewItems.Where(x => x.TargetType != ReviewTargetType.Section);
        var detailBySection = detailItems
            .GroupBy(x => x.Section)
            .ToDictionary(g => g.Key, g => g.ToList());

        var sections = ProfileApprovalFlow.Sections
            .OrderBy(s => (int)s)
            .Select(sec =>
            {
                var items = detailBySection.TryGetValue(sec, out var list) ? list : [];

                var secItem  = sectionItems.FirstOrDefault(x => x.Section == sec);

                return new SectionReviewDto
                {
                    Section = sec,
                    Status = ResolveStatus(items),
                    Note = secItem ?.ReviewerNote,
                    ReviewedAtUtc = secItem ?.ReviewedAtUtc ?? default
                };
            })
            .ToList();

        // ===== Profile data (snapshot) =====
        var profileData = MapProfile(profile);
        var dto = new GetProfileApprovalDetailDto
        {
            UserProfileId = profile.Id,
            UserId = profile.UserId,
            FullName = localization.GetLocalizedFullName(profile.User),
            CandidateType = localization.GetLocalizedName(profile.CandidateType),
            TargetEntity = localization.GetLocalizedName(profile.TargetEntity),
            Profile = profileData,
            Sections = sections
        };

        await auditRepo.AddAsync(new AuditTrailEntry
        {
            UserProfileId = profile.Id,
            UserId = request.OfficerId,
            ActionType = "OpenProfile",
            Notes = "Profile opened for review",
            Section = nameof(ProfileSection.Personal)
        });
        await uow.SaveChangesAsync(ct);

        return Result.Ok(dto);

        ReviewStatus ResolveStatus(IReadOnlyList<ReviewItem> items)
        {
            if (items.Any(i => i.Status == ReviewStatus.NeedsCorrection)) return ReviewStatus.NeedsCorrection;
            if (items.Any(i => i.Status == ReviewStatus.Rejected)) return ReviewStatus.Rejected;
            if (items.Any(i => i.Status is ReviewStatus.Pending or ReviewStatus.NotReviewed)) return ReviewStatus.Pending;
            return items.Count == 0 ? ReviewStatus.Pending : ReviewStatus.Approved;
        }

        // ===== Local helpers using mapper =====
        ProfileApprovalDataDto MapProfile(UserProfile profileEntity)
        {
            using var scope = new MapContextScope();
            scope.Context.Parameters[ResourceMapper.MediaKey] = media;

            var result = mapper.Map<ProfileApprovalDataDto>(profileEntity);
            result.Qualifications = profileEntity.Qualifications?
                .Select(mapper.Map<QualificationDto>)
                .ToList() ?? [];
            return result;
        }
    }
}
