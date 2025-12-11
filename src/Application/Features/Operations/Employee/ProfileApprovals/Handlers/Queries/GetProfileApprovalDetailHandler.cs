using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.DTOs;
using Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.Handlers.Queries;

public class GetProfileApprovalDetailHandler(IUnitOfWork uow, IMapper mapper, IMediaUrlResolver media)
    : IRequestHandler<GetProfileApprovalDetailQuery, Result<ProfileApprovalDetailDto>>
{
    public async Task<Result<ProfileApprovalDetailDto>> Handle(GetProfileApprovalDetailQuery request, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();

        var profile = await profileRepo.DbSet
            .Include(p => p.User)
            .Include(p => p.SponsorProfile!.SponsorCard)
            .Include(p => p.SponsorProfile!.SponsorType)
            .Include(p => p.Office)
            .Include(p => p.BirthdayCertificate)
            .Include(p => p.MarriageCertificate)
            .Include(p => p.ResidenceAddress)
            .Include(p => p.CandidateType)
            .Include(p => p.TargetEntity)
            .Include(p => p.Nationality)
            .Include(p => p.Gender)
            .Include(p => p.Religion)
            .Include(p => p.MaritalStatus)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.Major)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.University)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.Certificate)
            .Include(p => p.Experiences)!.ThenInclude(e => e.Certificate)
            .Include(p => p.TrainingCourses)!.ThenInclude(t => t.Certificate)
            .Include(p => p.Achievements)!.ThenInclude(a => a.AchievementType)
            .Include(p => p.Achievements)!.ThenInclude(a => a.Attachment)
            .Include(p => p.Skills)!.ThenInclude(s => s.Skill)
            .Include(p => p.Languages)!.ThenInclude(l => l.Language!)
            .Include(p => p.AdditionalAttachments)!.ThenInclude(a => a.Attachment)
            .FirstOrDefaultAsync(p => p.Id == request.UserProfileId, ct);

        if (profile is null)
            return Result.Fail<ProfileApprovalDetailDto>(ErrorsCodes.UserProfileNotFound);

        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();
        var auditRepo = uow.GetEntityRepository<AuditTrailEntry>();

        var isAssigned = await assignmentRepo.DbSet
            .AnyAsync(a => a.UserProfileId == profile.Id && a.EmployeeId == request.OfficerId && a.IsActive, ct);

        if (!isAssigned)
            return Result.Fail<ProfileApprovalDetailDto>(ErrorsCodes.UnauthorizedAction);

        var submissionRepo = uow.GetEntityRepository<ProfileSubmission>();
        var submission = await submissionRepo.DbSet
            .Where(s => s.UserProfileId == profile.Id)
            .OrderByDescending(s => s.Version)
            .FirstOrDefaultAsync(ct);

        if (profile.Status is not UserProfileStatus.Approved
            and not UserProfileStatus.Rejected
            and not UserProfileStatus.AdminCancelled)
        {
            profile.Status = UserProfileStatus.UnderReview;
            await uow.SaveChangesAsync(ct);
        }

        // ===== Reviews =====
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        var reviewItems = await reviewRepo.DbSet
            .Where(r => r.UserProfileId == profile.Id)
            .OrderByDescending(r => r.Version)
            .ToListAsync(ct);

        var latestItems = reviewItems
            .GroupBy(r => new { r.TargetType, r.Section, r.FieldPath, r.EntityName, r.EntityId, r.ResourceId })
            .Select(g => g.First())
            .ToList();

        var resourceIds = latestItems
            .Where(r => r.ResourceId.HasValue)
            .Select(r => r.ResourceId!.Value)
            .Distinct()
            .ToList();

        var resourceRepo = uow.GetEntityRepository<Resource>();
        var resources = await resourceRepo.DbSet
            .Where(r => resourceIds.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id, ct);

        // Map ReviewItem -> DTOs using mapper
        var reviewItemDtos = latestItems
            .Select(item =>
            {
                var dto = mapper.Map<ProfileApprovalItemDto>(item);

                if (item.ResourceId.HasValue &&
                    resources.TryGetValue(item.ResourceId.Value, out var resource))
                {
                    dto.ResourceUrl = media.ResolveAbsolute(resource.Url);
                }

                return (Item: item, Dto: dto);
            })
            .ToList();

        var sections = reviewItemDtos
            .GroupBy(x => x.Item.Section)
            .Select(group =>
            {
                var sectionReview = group
                    .Where(x => x.Item.TargetType == ReviewTargetType.Section)
                    .Select(x => x.Dto)
                    .FirstOrDefault();

                var entries = group
                    .Where(x => x.Item.TargetType != ReviewTargetType.Section)
                    .Select(x => x.Dto)
                    .ToList();

                return new ProfileApprovalSectionDto
                {
                    Section = group.Key,
                    SectionReview = sectionReview,
                    Items = entries,
                    HasAttachments = entries.Any(e => e.TargetType == ReviewTargetType.Attachment)
                };
            })
            .OrderBy(s => (int)s.Section)
            .ToList();

        // ===== Profile data (snapshot) =====
        var profileData = MapProfile(profile);

        var dto = new ProfileApprovalDetailDto
        {
            UserProfileId = profile.Id,
            UserId = profile.UserId,
            FullName = profile.User?.FullNameEn ?? profile.User?.FullNameAr ?? string.Empty,
            CandidateType = profile.CandidateType?.NameAr ?? profile.CandidateType?.NameEn,
            TargetEntity = profile.TargetEntity?.NameAr ?? profile.TargetEntity?.NameEn,
            SubmissionVersion = submission?.Version,
            SubmittedAtUtc = submission?.SubmittedAtUtc,
            Profile = profileData,
            Sections = sections
        };

        await auditRepo.AddAsync(new AuditTrailEntry
        {
            UserProfileId = profile.Id,
            UserId = request.OfficerId,
            ActionType = "OpenProfile",
            Notes = "Profile opened for review",
            Section = nameof(ProfileSection.BasicInformation)
        });
        await uow.SaveChangesAsync(ct);

        return Result.Ok(dto);

        // ===== Local helpers using mapper =====

        ProfileApprovalDataDto MapProfile(UserProfile profileEntity)
        {
            var result = mapper.Map<ProfileApprovalDataDto>(profileEntity);
            result.Qualifications = profileEntity.Qualifications?
                                        .OrderBy(q => q.GraduationYear)
                                        .Select(mapper.Map<QualificationDto>)
                                        .ToList() ?? [];

            result.Experiences = profileEntity.Experiences?
                                     .Select(mapper.Map<ExperienceDto>)
                                     .ToList() ?? [];

            result.TrainingCourses = profileEntity.TrainingCourses?
                                         .Select(mapper.Map<TrainingCourseDto>)
                                         .ToList() ?? [];

            result.ProfessionalCertificatesAndAwards = profileEntity.Achievements?
                                                           .Select(mapper.Map<AchievementDto>)
                                                           .ToList() ?? [];

            result.Attachments = profileEntity.AdditionalAttachments?
                                     .Where(a => a.Attachment != null)
                                     .Select(mapper.Map<AdditionalAttachmentDto>)
                                     .ToList() ?? [];

            result.ProfilePhoto = string.IsNullOrWhiteSpace(profileEntity.User?.Avatar)
                ? null
                : new FileRefDto
                {
                    ResourceId = Guid.Empty,
                    FileName = "profile-photo",
                    Url = media.ResolveAbsolute(profileEntity.User!.Avatar!)
                };
            return result;
        }
    }
}
