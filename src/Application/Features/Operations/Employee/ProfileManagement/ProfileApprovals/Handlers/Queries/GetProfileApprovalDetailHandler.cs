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

public class GetProfileApprovalDetailHandler(IUnitOfWork uow, IMapper mapper, IMediaUrlResolver media)
    : IRequestHandler<GetProfileApprovalDetailQuery, Result<GetProfileApprovalDetailDto>>
{
    public async Task<Result<GetProfileApprovalDetailDto>> Handle(GetProfileApprovalDetailQuery request,
        CancellationToken ct)
    {
        
        var profile = await UserProfileLoader.GetFullProfile(uow, request.UserProfileId, ct);
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

        var sectionItems = await reviewRepo.DbSet
            .AsNoTracking()
            .Where(r => r.UserProfileId == profile.Id &&
                        r.TargetType == ReviewTargetType.Section)
            .ToListAsync(ct);

        var sections = ProfileApprovalFlow.Sections
            .OrderBy(s => (int)s)
            .Select(sec =>
            {
                var item = sectionItems.FirstOrDefault(x => x.Section == sec);

                return new SectionReviewDto
                {
                    Section = sec,
                    Status = item?.Status ?? ReviewStatus.Pending,
                    Note = item?.ReviewerNote,
                    ReviewedAtUtc = item?.ReviewedAtUtc ?? default
                };
            })
            .ToList();

        // ===== Profile data (snapshot) =====
        var profileData = MapProfile(profile);
        var dto = new GetProfileApprovalDetailDto
        {
            UserProfileId = profile.Id,
            UserId = profile.UserId,
            FullName = profile.User?.FullNameEn ?? profile.User?.FullNameAr ?? string.Empty,
            CandidateType = profile.CandidateType?.NameAr ?? profile.CandidateType?.NameEn,
            TargetEntity = profile.TargetEntity?.NameAr ?? profile.TargetEntity?.NameEn,
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
