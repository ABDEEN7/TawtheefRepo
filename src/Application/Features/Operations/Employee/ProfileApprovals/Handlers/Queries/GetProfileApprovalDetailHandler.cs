using FluentResults;
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

public class GetProfileApprovalDetailHandler(IUnitOfWork uow, IMediaUrlResolver media)
    : IRequestHandler<GetProfileApprovalDetailQuery, Result<ProfileApprovalDetailDto>>
{
    public async Task<Result<ProfileApprovalDetailDto>> Handle(GetProfileApprovalDetailQuery request, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();

        var profile = await profileRepo.DbSet
            .Include(p => p.User)
            .Include(p => p.CandidateType)
            .Include(p => p.TargetEntity)
            .Include(p => p.Nationality)
            .Include(p => p.Gender)
            .Include(p => p.Religion)
            .Include(p => p.MaritalStatus)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.Major)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.University)
            .Include(p => p.Experiences)!.ThenInclude(e => e.Certificate)
            .Include(p => p.TrainingCourses)!.ThenInclude(t => t.Certificate)
            .Include(p => p.Achievements)!.ThenInclude(a => a.Attachment)
            .Include(p => p.Skills)!.ThenInclude(s => s.Skill)
            .Include(p => p.Languages)!.ThenInclude(l => l.Language)
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

        if (profile.Status is not UserProfileStatus.Approved and not UserProfileStatus.Rejected and not UserProfileStatus.AdminCancelled)
        {
            profile.Status = UserProfileStatus.UnderReview;
            await uow.SaveChangesAsync(ct);
        }

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

        var sections = latestItems
            .GroupBy(r => r.Section)
            .Select(group =>
            {
                var sectionReview = group.FirstOrDefault(i => i.TargetType == ReviewTargetType.Section);
                var entries = group
                    .Where(i => i.TargetType != ReviewTargetType.Section)
                    .Select(MapItem)
                    .ToList();

                return new ProfileApprovalSectionDto
                {
                    Section = group.Key,
                    SectionReview = sectionReview is null ? null : MapItem(sectionReview),
                    Items = entries,
                    HasAttachments = entries.Any(e => e.TargetType == ReviewTargetType.Attachment)
                };
            })
            .OrderBy(s => (int)s.Section)
            .ToList();

        var dto = new ProfileApprovalDetailDto
        {
            UserProfileId = profile.Id,
            UserId = profile.UserId,
            FullName = profile.User?.FullNameEn ?? profile.User?.FullNameAr ?? string.Empty,
            CandidateType = profile.CandidateType?.NameAr ?? profile.CandidateType?.NameEn,
            TargetEntity = profile.TargetEntity?.NameAr ?? profile.TargetEntity?.NameEn,
            SubmissionVersion = submission?.Version,
            SubmittedAtUtc = submission?.SubmittedAtUtc,
            Profile = MapProfile(),
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

        ProfileApprovalItemDto MapItem(ReviewItem item)
        {
            var title = item.TargetType switch
            {
                ReviewTargetType.Section => "Textual data",
                ReviewTargetType.Attachment => item.AttachmentTitle ?? "Attachment",
                ReviewTargetType.Row => item.EntityName ?? "Row",
                ReviewTargetType.Field => item.FieldPath ?? "Field",
                _ => "Review item"
            };

            string? resourceUrl = null;
            if (item.ResourceId.HasValue && resources.TryGetValue(item.ResourceId.Value, out var resource))
            {
                resourceUrl = media.ResolveAbsolute(resource.Url);
            }

            return new ProfileApprovalItemDto
            {
                ReviewItemId = item.Id,
                TargetType = item.TargetType,
                Status = item.Status,
                Title = title,
                Note = item.ReviewerNote,
                ResourceId = item.ResourceId,
                ResourceUrl = resourceUrl,
                EntityId = item.EntityId,
                EntityName = item.EntityName,
                Version = item.Version,
                ApprovedAtVersion = item.ApprovedAtVersion,
                ReviewedAtUtc = item.ReviewedAtUtc
            };
        }

        ProfileApprovalDataDto MapProfile()
        {
            FileRefDto? MapFile(Resource? resource)
            {
                if (resource is null) return null;
                return new FileRefDto
                {
                    ResourceId = resource.Id,
                    FileName = resource.Name,
                    Url = media.ResolveAbsolute(resource.Url)
                };
            }

            return new ProfileApprovalDataDto
            {
                BasicInformation = new BasicInformationSnapshot
                {
                    FullNameAr = profile.User?.FullNameAr,
                    FullNameEn = profile.User?.FullNameEn,
                    NationalNumber = profile.NationalNumber,
                    BirthDate = profile.BirthDate,
                    Nationality = profile.Nationality?.NameAr ?? profile.Nationality?.NameEn,
                    Gender = profile.Gender?.NameAr ?? profile.Gender?.NameEn,
                    Religion = profile.Religion?.NameAr ?? profile.Religion?.NameEn,
                    MaritalStatus = profile.MaritalStatus?.NameAr ?? profile.MaritalStatus?.NameEn,
                    ChildrenCount = profile.ChildrenCount,
                    CandidateType = profile.CandidateType?.NameAr ?? profile.CandidateType?.NameEn,
                    TargetEntity = profile.TargetEntity?.NameAr ?? profile.TargetEntity?.NameEn,
                    ResumeAttachment = MapFile(profile.ResumeAttachment),
                    NationalCard = MapFile(profile.NationalCard),
                    ResidenceAddressCertificate = MapFile(profile.ResidenceAddressCertificate),
                    BirthdayCertificate = MapFile(profile.BirthdayCertificate),
                    MarriageCertificate = MapFile(profile.MarriageCertificate)
                },
                Qualifications = profile.Qualifications?.Select(q => new QualificationDto
                {
                    Id = q.Id,
                    DegreeId = q.DegreeId,
                    GradCountryId = q.CountryId,
                    MajorId = q.MajorId,
                    SubMajorId = q.SubMajorId,
                    UniversityId = q.UniversityId,
                    StudyTypeId = q.StudyTypeId,
                    GradeId = q.RatingId,
                    GraduationYear = q.GraduationYear,
                    Gpa = q.GPA,
                    Attachment = MapFile(q.Certificate)
                }).ToList() ?? [],
                Experiences = profile.Experiences?.Select(e => new ExperienceDto
                {
                    Id = e.Id,
                    Description = e.Description,
                    EmployerName = e.EmployerName,
                    JobTitle = e.JobTitle,
                    CountryId = e.CountryId,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    IsCurrent = e.EndDate == null,
                    QualificationId = e.QualificationId,
                    Attachment = MapFile(e.Certificate)
                }).ToList() ?? [],
                TrainingCourses = profile.TrainingCourses?.Select(t => new TrainingCourseDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Provider = t.Provider,
                    CountryId = t.CountryId,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    Description = t.Description,
                    Attachment = MapFile(t.Certificate)
                }).ToList() ?? [],
                ProfessionalCertificatesAndAwards = profile.Achievements?.Select(a => new AchievementDto
                {
                    Id = a.Id,
                    AchievementTypeId = a.AchievementTypeId,
                    Title = a.Title,
                    IssuingAuthority = a.IssuingAuthority,
                    CountryId = a.CountryId,
                    IssuedDate = a.IssuedDate,
                    RelatedToSpecialization = a.RelatedToSpecialization,
                    Attachment = MapFile(a.Attachment)
                }).ToList() ?? [],
                SkillsAndLanguages = profile.Skills?.Select(s => new SkillDto
                {
                    Id = s.Id,
                    SkillId = s.SkillId,
                    Skill = s.Skill
                }).ToList() ?? [],
                Languages = profile.Languages?.Select(l => new LanguageDto
                {
                    Id = l.Id,
                    LanguageId = l.LanguageId,
                    Language = l.Language
                }).ToList() ?? [],
                Attachments = profile.AdditionalAttachments?.Where(a => a.Attachment != null).Select(a => new AdditionalAttachmentDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    File = MapFile(a.Attachment)
                }).ToList() ?? [],
                ProfilePhoto = string.IsNullOrWhiteSpace(profile.User?.Avatar)
                    ? null
                    : new FileRefDto
                    {
                        ResourceId = Guid.Empty,
                        FileName = "profile-photo",
                        Url = media.ResolveAbsolute(profile.User!.Avatar!)
                    }
            };
        }
    }
}
