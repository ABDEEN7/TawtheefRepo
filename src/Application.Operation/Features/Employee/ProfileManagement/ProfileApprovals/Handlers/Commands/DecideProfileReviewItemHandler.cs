using System.Text.Json;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Commands;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.SaveOperation;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.Spanshot;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Handlers.Commands;

public sealed class DecideProfileReviewItemHandler(IUnitOfWork uow, TimeProvider time)
    : IRequestHandler<DecideProfileReviewItemCommand, IResult<Unit>>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IResult<Unit>> Handle(DecideProfileReviewItemCommand cmd, CancellationToken ct)
    {
        if (cmd.OfficerId == Guid.Empty)
            return Result.Fail<Unit>(ErrorsCodes.InvalidUserIdentifier);

        if (cmd.Status is not (ReviewStatus.Approved or ReviewStatus.Rejected or ReviewStatus.NeedsCorrection))
            return Result.Fail<Unit>(ErrorsCodes.UnExpectedError);

        if (cmd.Status is ReviewStatus.Rejected or ReviewStatus.NeedsCorrection &&
            string.IsNullOrWhiteSpace(cmd.Note))
            return Result.Fail<Unit>(ErrorsCodes.NotesRequiredForCorrection);

        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        var auditRepo = uow.GetEntityRepository<AuditTrailEntry>();
        var loggerRepo = uow.GetEntityRepository<UserProfileLogger>();

        var item = await reviewRepo.DbSet
            .Include(r => r.ProfileChange)
            .FirstOrDefaultAsync(r => r.Id == cmd.ReviewItemId && !r.IsDeleted, ct);

        if (item is null)
            return Result.Fail<Unit>(ErrorsCodes.ReviewItemNotFound);

        if (cmd.Status == ReviewStatus.Approved && 
            (item.EntityName == ProfileReviewConstants.EntityNames.Experience || item.EntityName == ProfileReviewConstants.EntityNames.TrainingCourse) &&
            cmd.SpecializationRelation == null)
        {
            return Result.Fail<Unit>(ErrorsCodes.SpecializationRelationRequired);
        }

        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();
        var isAssigned = await assignmentRepo.DbSet
            .AsNoTracking()
            .AnyAsync(a => a.IsActive && a.UserProfileId == item.UserProfileId && a.EmployeeId == cmd.OfficerId, ct);

        if (!isAssigned)
            return Result.Fail<Unit>(ErrorsCodes.UnauthorizedAction);

        var change = item.ProfileChange;
        var hasChangeRequest = change is not null;

        if (cmd.Status == ReviewStatus.Approved && item.Status != ReviewStatus.Approved && hasChangeRequest)
        {
            var profileRepo = uow.GetEntityRepository<UserProfile>();
            var profile = await profileRepo.DbSet
                .Include(p => p.User)
                .Include(p => p.SponsorProfile)
                .Include(p => p.ResidenceAddress)
                .Include(p => p.Qualifications)
                .Include(p => p.Experiences)
                .Include(p => p.TrainingCourses)
                .Include(p => p.Achievements)
                .Include(p => p.Skills)
                .Include(p => p.Languages)
                .Include(p => p.AdditionalAttachments)
                .FirstOrDefaultAsync(p => p.Id == item.UserProfileId, ct);

            if (profile is null)
                return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

            var applyResult = ApplyChange(profile, item, change!);
            if (applyResult.IsFailed)
                return Result.Fail<Unit>(applyResult.Errors);
        }

        var now = time.GetUtcNow().UtcDateTime;

        item.Status = cmd.Status;
        item.ReviewerNote = cmd.Note;
        item.ReviewedById = cmd.OfficerId;
        item.ReviewedAtUtc = now;
        if (cmd.Status == ReviewStatus.Approved)
            item.ApprovedHash = item.CurrentHash;

        item.IsOutdated = false;

        if (hasChangeRequest)
        {
            change!.Status = cmd.Status == ReviewStatus.Approved
                ? ProfileChangeRequestStatus.Approved
                : ProfileChangeRequestStatus.Rejected;
            change.ReviewedById = cmd.OfficerId;
            change.ReviewedAtUtc = now;
            change.ReviewerNote = cmd.Note;
        }

        await auditRepo.AddAsync(new AuditTrailEntry
        {
            UserProfileId = item.UserProfileId,
            UserId = cmd.OfficerId,
            ActionType = UserProfileLogConstants.ActionTypes.ReviewItemDecision,
            Notes = $"Review item {item.Id} marked {cmd.Status}",
            Section = item.Section.ToString(),
            EntityId = item.EntityId ?? item.Id,
            AttachmentId = item.ResourceId
        });

        await loggerRepo.AddAsync(new UserProfileLogger
        {
            UserProfileId = item.UserProfileId,
            PerformedById = cmd.OfficerId,
            ActionType = UserProfileLogConstants.ActionTypes.ReviewItemDecision,
            Notes = $"Review item {item.Id} marked {cmd.Status}",
            Section = item.Section.ToString(),
            EntityId = item.EntityId ?? item.Id,
            AttachmentId = item.ResourceId,
            ReviewStatus = cmd.Status
        });

        if (cmd.Status == ReviewStatus.Approved)
        {
            await UpdateEntityRelevance(item, cmd.SpecializationRelation, ct);
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }

    private async Task UpdateEntityRelevance(ReviewItem item, SpecializationRelationLevel? relevance, CancellationToken ct)
    {
        if (relevance == null) return;

        if (item.EntityName == ProfileReviewConstants.EntityNames.Experience)
        {
            var repo = uow.GetEntityRepository<Experience>();
            var entity = await repo.DbSet.FirstOrDefaultAsync(e => e.Id == item.EntityId, ct);
            if (entity != null)
            {
                entity.SpecializationRelation = relevance;
            }
        }
        else if (item.EntityName == ProfileReviewConstants.EntityNames.TrainingCourse)
        {
            var repo = uow.GetEntityRepository<TrainingCourse>();
            var entity = await repo.DbSet.FirstOrDefaultAsync(t => t.Id == item.EntityId, ct);
            if (entity != null)
            {
                entity.SpecializationRelation = relevance;
            }
        }
    }

    private static Result ApplyChange(UserProfile profile, ReviewItem item, ProfileChangeRequest change)
    {
        if (string.IsNullOrWhiteSpace(change.NewValue))
            return Result.Fail(ErrorsCodes.UnExpectedError);

        return item.TargetType switch
        {
            ReviewTargetType.Section => ApplySectionChange(profile, item.Section, change.NewValue),
            ReviewTargetType.Row => ApplyRowChange(profile, item.Section, item.EntityName, item.EntityId, change.NewValue),
            _ => Result.Fail(ErrorsCodes.UnExpectedError)
        };
    }

    private static Result ApplySectionChange(UserProfile profile, ProfileSection section, string newValueJson)
    {
        return section switch
        {
            ProfileSection.Prerequisites => ApplyPrerequisites(profile, newValueJson),
            ProfileSection.Personal => ApplyPersonal(profile, newValueJson),
            ProfileSection.Contact => ApplyContact(profile, newValueJson),
            _ => Result.Fail(ErrorsCodes.UnExpectedError)
        };
    }

    private static Result ApplyRowChange(
        UserProfile profile,
        ProfileSection section,
        string? entityName,
        Guid? entityId,
        string newValueJson)
    {
        if (entityId is null || entityId == Guid.Empty)
            return Result.Fail(ErrorsCodes.UnExpectedError);

        return (section, entityName) switch
        {
            (ProfileSection.Qualifications, ProfileReviewConstants.EntityNames.Qualification) => AddQualification(profile, entityId.Value, newValueJson),
            (ProfileSection.Experience, ProfileReviewConstants.EntityNames.Experience) => AddExperience(profile, entityId.Value, newValueJson),
            (ProfileSection.TrainingCourses, ProfileReviewConstants.EntityNames.TrainingCourse) => AddTrainingCourse(profile, entityId.Value, newValueJson),
            (ProfileSection.CertificatesAndAwards, ProfileReviewConstants.EntityNames.Achievement) => AddAchievement(profile, entityId.Value, newValueJson),
            (ProfileSection.Skills, ProfileReviewConstants.EntityNames.Skill) => AddSkill(profile, entityId.Value, newValueJson),
            (ProfileSection.Languages, ProfileReviewConstants.EntityNames.Language) => AddLanguage(profile, entityId.Value, newValueJson),
            (ProfileSection.Attachments, ProfileReviewConstants.EntityNames.Attachment) => AddAdditionalAttachment(profile, entityId.Value, newValueJson),
            _ => Result.Fail(ErrorsCodes.UnExpectedError)
        };
    }

    private static Result ApplyPrerequisites(UserProfile profile, string json)
    {
        var snapshotResult = Deserialize<PrereqSnapshot>(json);
        if (snapshotResult.IsFailed)
            return Result.Fail(snapshotResult.Errors);

        var s = snapshotResult.Value;
        if (s.CandidateTypeId is null || s.CandidateTypeId == Guid.Empty)
            return Result.Fail(ErrorsCodes.UnExpectedError);
        if (s.TargetEntityId is null || s.TargetEntityId == Guid.Empty)
            return Result.Fail(ErrorsCodes.UnExpectedError);

        profile.CandidateTypeId = s.CandidateTypeId.Value;
        profile.TargetEntityId = s.TargetEntityId.Value;
        profile.OfficeId = s.OfficeId;
        profile.QIDExpiry = s.QidExpiry;
        profile.ResumeAttachmentId = s.ResumeAttachmentId;
        profile.NationalCardId = s.NationalCardId;
        profile.BirthdayCertificateId = s.BirthCertificateId;
        profile.MarriageCertificateId = s.MarriageCertificateId;

        return Result.Ok();
    }

    private static Result ApplyPersonal(UserProfile profile, string json)
    {
        var snapshotResult = Deserialize<PersonalSnapshot>(json);
        if (snapshotResult.IsFailed)
            return Result.Fail(snapshotResult.Errors);

        var s = snapshotResult.Value;

        if (profile.User is not null)
        {
            if (!string.IsNullOrWhiteSpace(s.FullNameAr))
                profile.User.FullNameAr = s.FullNameAr;
            if (!string.IsNullOrWhiteSpace(s.FullNameEn))
                profile.User.FullNameEn = s.FullNameEn;
        }

        profile.NationalNumber = s.NationalNumber;
        profile.QIDExpiry = s.QidExpiry;
        profile.BirthDate = s.BirthDate;
        profile.NationalityId = s.NationalityId;
        profile.GenderId = s.GenderId;
        profile.ReligionId = s.ReligionId;
        profile.MaritalStatusId = s.MaritalStatusId;
        profile.ChildrenCount = s.ChildrenCount ?? profile.ChildrenCount;
        profile.HasDisability = s.HasDisability;
        profile.DisabilityDetails = s.HasDisability ? s.DisabilityDetails : null;

        if (!string.IsNullOrWhiteSpace(s.SponsorEmployerName) &&
            !string.IsNullOrWhiteSpace(s.SponsorEmployerNumber) &&
            s.SponsorTypeId.HasValue &&
            s.SponsorTypeId != Guid.Empty)
        {
            if (!s.SponsorQidExpiry.HasValue)
                return Result.Fail(ErrorsCodes.UnExpectedError);

            profile.SponsorProfile ??= new SponsorProfile
            {
                SponsorTypeId = s.SponsorTypeId.Value,
                SponsorName = s.SponsorEmployerName,
                SponsorNumber = s.SponsorEmployerNumber,
                QIDExpiry = s.SponsorQidExpiry.Value,
                SponsorCardId = s.SponsorCardResourceId
            };

            profile.SponsorProfile.SponsorTypeId = s.SponsorTypeId.Value;
            profile.SponsorProfile.SponsorName = s.SponsorEmployerName;
            profile.SponsorProfile.SponsorNumber = s.SponsorEmployerNumber;
            profile.SponsorProfile.QIDExpiry = s.SponsorQidExpiry.Value;
            profile.SponsorProfile.SponsorCardId = s.SponsorCardResourceId;
        }

        return Result.Ok();
    }

    private static Result ApplyContact(UserProfile profile, string json)
    {
        var snapshotResult = Deserialize<ContactSnapshot>(json);
        if (snapshotResult.IsFailed)
            return Result.Fail(snapshotResult.Errors);

        var s = snapshotResult.Value;
        profile.ResidenceCountryId = s.ResidenceCountryId;
        profile.InterviewLocationId = s.InterviewLocationId;
        profile.Address = s.Address;

        var hasNationalAddress = s.Zone.HasValue || s.Street.HasValue || s.Building.HasValue || s.Unit.HasValue || s.NationalAddressCertificateId.HasValue;
        if (!hasNationalAddress)
            return Result.Ok();

        if (!s.Zone.HasValue || !s.Street.HasValue || !s.Building.HasValue || !s.Unit.HasValue || !s.NationalAddressCertificateId.HasValue)
            return Result.Fail(ErrorsCodes.UnExpectedError);

        profile.ResidenceAddress ??= new ResidenceAddress();
        profile.ResidenceAddress.ZoneNo = s.Zone.Value;
        profile.ResidenceAddress.StreetNo = s.Street.Value;
        profile.ResidenceAddress.BuildingNo = s.Building.Value;
        profile.ResidenceAddress.UnitNo = s.Unit.Value;
        profile.ResidenceAddress.CertificateId = s.NationalAddressCertificateId.Value;

        return Result.Ok();
    }

    private static Result AddQualification(UserProfile profile, Guid entityId, string json)
    {
        var snapshotResult = Deserialize<PendingQualificationSnapshot>(json);
        if (snapshotResult.IsFailed)
            return Result.Fail(snapshotResult.Errors);

        var s = snapshotResult.Value;
        if (s.DegreeId is null || s.DegreeId == Guid.Empty)
            return Result.Fail(ErrorsCodes.UnExpectedError);
        if (s.GradCountryId is null || s.GradCountryId == Guid.Empty)
            return Result.Fail(ErrorsCodes.UnExpectedError);

        profile.Qualifications ??= [];
        if (profile.Qualifications.Any(q => q.Id == entityId))
            return Result.Fail(ErrorsCodes.UnExpectedError);

        profile.Qualifications.Add(new Qualification
        {
            Id = entityId,
            UserProfileId = profile.Id,
            DegreeId = s.DegreeId.Value,
            CountryId = s.GradCountryId.Value,
            UniversityId = s.UniversityId,
            MajorId = s.MajorId,
            SubMajorId = s.SubMajorId,
            StudyTypeId = s.StudyTypeId,
            RatingId = s.GradeId,
            GraduationYear = s.GradYear,
            GPA = s.Gpa,
            CertificateId = s.AttachmentResourceId
        });

        return Result.Ok();
    }

    private static Result AddExperience(UserProfile profile, Guid entityId, string json)
    {
        var snapshotResult = Deserialize<PendingExperienceSnapshot>(json);
        if (snapshotResult.IsFailed)
            return Result.Fail(snapshotResult.Errors);

        var s = snapshotResult.Value;
        if (string.IsNullOrWhiteSpace(s.EmployerName))
            return Result.Fail(ErrorsCodes.UnExpectedError);
        if (string.IsNullOrWhiteSpace(s.JobTitle))
            return Result.Fail(ErrorsCodes.UnExpectedError);
        if (!s.StartDate.HasValue)
            return Result.Fail(ErrorsCodes.UnExpectedError);
        if (s.CountryId is null || s.CountryId == Guid.Empty)
            return Result.Fail(ErrorsCodes.UnExpectedError);

        profile.Experiences ??= [];
        if (profile.Experiences.Any(e => e.Id == entityId))
            return Result.Fail(ErrorsCodes.UnExpectedError);

        profile.Experiences.Add(new Experience
        {
            Id = entityId,
            UserProfileId = profile.Id,
            EmployerName = s.EmployerName,
            JobTitle = s.JobTitle,
            StartDate = s.StartDate.Value,
            EndDate = s.EndDate,
            CountryId = s.CountryId.Value,
            CertificateId = s.CertificateId ?? Guid.Empty,
            Description = s.Description,
            QualificationId = s.QualificationId
        });

        return Result.Ok();
    }

    private static Result AddTrainingCourse(UserProfile profile, Guid entityId, string json)
    {
        var snapshotResult = Deserialize<PendingTrainingSnapshot>(json);
        if (snapshotResult.IsFailed)
            return Result.Fail(snapshotResult.Errors);

        var s = snapshotResult.Value;
        if (string.IsNullOrWhiteSpace(s.Title))
            return Result.Fail(ErrorsCodes.UnExpectedError);
        if (string.IsNullOrWhiteSpace(s.Provider))
            return Result.Fail(ErrorsCodes.UnExpectedError);
        if (!s.StartDate.HasValue)
            return Result.Fail(ErrorsCodes.UnExpectedError);
        if (s.CountryId is null || s.CountryId == Guid.Empty)
            return Result.Fail(ErrorsCodes.UnExpectedError);

        profile.TrainingCourses ??= [];
        if (profile.TrainingCourses.Any(t => t.Id == entityId))
            return Result.Fail(ErrorsCodes.UnExpectedError);

        profile.TrainingCourses.Add(new TrainingCourse
        {
            Id = entityId,
            UserProfileId = profile.Id,
            Title = s.Title,
            Provider = s.Provider,
            StartDate = s.StartDate.Value,
            EndDate = s.EndDate,
            CountryId = s.CountryId.Value,
            CertificateId = s.CertificateId ?? Guid.Empty,
            Description = s.Description
        });

        return Result.Ok();
    }

    private static Result AddAchievement(UserProfile profile, Guid entityId, string json)
    {
        var snapshotResult = Deserialize<PendingAchievementSnapshot>(json);
        if (snapshotResult.IsFailed)
            return Result.Fail(snapshotResult.Errors);

        var s = snapshotResult.Value;
        if (s.AchievementTypeId == Guid.Empty)
            return Result.Fail(ErrorsCodes.UnExpectedError);
        if (string.IsNullOrWhiteSpace(s.Title))
            return Result.Fail(ErrorsCodes.UnExpectedError);
        if (string.IsNullOrWhiteSpace(s.IssuingAuthority))
            return Result.Fail(ErrorsCodes.UnExpectedError);
        if (s.CountryId is null || s.CountryId == Guid.Empty)
            return Result.Fail(ErrorsCodes.UnExpectedError);
        if (!s.IssueDate.HasValue)
            return Result.Fail(ErrorsCodes.UnExpectedError);

        profile.Achievements ??= [];
        if (profile.Achievements.Any(a => a.Id == entityId))
            return Result.Fail(ErrorsCodes.UnExpectedError);

        profile.Achievements.Add(new Achievement
        {
            Id = entityId,
            UserProfileId = profile.Id,
            AchievementTypeId = s.AchievementTypeId,
            Title = s.Title,
            IssuingAuthority = s.IssuingAuthority,
            CountryId = s.CountryId.Value,
            IssueDate = s.IssueDate.Value,
            Description = s.Description,
            RelatedToSpecialization = s.RelatedToSpecialization,
            AttachmentId = s.AttachmentResourceId ?? Guid.Empty
        });

        return Result.Ok();
    }

    private static Result AddSkill(UserProfile profile, Guid entityId, string json)
    {
        var dtoResult = Deserialize<SkillUpsertDto>(json);
        if (dtoResult.IsFailed)
            return Result.Fail(dtoResult.Errors);

        var dto = dtoResult.Value;
        if (dto.SkillId == Guid.Empty || dto.LevelId == Guid.Empty)
            return Result.Fail(ErrorsCodes.UnExpectedError);

        profile.Skills ??= [];
        if (profile.Skills.Any(s => s.Id == entityId))
            return Result.Fail(ErrorsCodes.UnExpectedError);

        profile.Skills.Add(new ProfileSkill
        {
            Id = entityId,
            UserProfileId = profile.Id,
            SkillId = dto.SkillId,
            LevelId = dto.LevelId
        });

        return Result.Ok();
    }

    private static Result AddLanguage(UserProfile profile, Guid entityId, string json)
    {
        var dtoResult = Deserialize<ProfileLanguageUpsertDto>(json);
        if (dtoResult.IsFailed)
            return Result.Fail(dtoResult.Errors);

        var dto = dtoResult.Value;
        if (dto.LanguageId == Guid.Empty ||
            dto.SpeakingLevelId == Guid.Empty ||
            dto.WritingLevelId == Guid.Empty ||
            dto.ReadingLevelId == Guid.Empty)
            return Result.Fail(ErrorsCodes.UnExpectedError);

        profile.Languages ??= [];
        if (profile.Languages.Any(l => l.Id == entityId))
            return Result.Fail(ErrorsCodes.UnExpectedError);

        profile.Languages.Add(new ProfileLanguage
        {
            Id = entityId,
            UserProfileId = profile.Id,
            LanguageId = dto.LanguageId,
            SpeakingLevelId = dto.SpeakingLevelId,
            WritingLevelId = dto.WritingLevelId,
            ReadingLevelId = dto.ReadingLevelId
        });

        return Result.Ok();
    }

    private static Result AddAdditionalAttachment(UserProfile profile, Guid entityId, string json)
    {
        var snapshotResult = Deserialize<PendingAttachmentSnapshot>(json);
        if (snapshotResult.IsFailed)
            return Result.Fail(snapshotResult.Errors);

        var s = snapshotResult.Value;
        if (string.IsNullOrWhiteSpace(s.Title))
            return Result.Fail(ErrorsCodes.UnExpectedError);
        if (s.AttachmentResourceId is null || s.AttachmentResourceId == Guid.Empty)
            return Result.Fail(ErrorsCodes.UnExpectedError);

        profile.AdditionalAttachments ??= [];
        if (profile.AdditionalAttachments.Any(a => a.Id == entityId))
            return Result.Fail(ErrorsCodes.UnExpectedError);

        profile.AdditionalAttachments.Add(new ProfileAdditionalAttachment
        {
            Id = entityId,
            UserProfileId = profile.Id,
            FileName = s.Title,
            AttachmentId = s.AttachmentResourceId.Value
        });

        return Result.Ok();
    }

    private static Result<T> Deserialize<T>(string json)
    {
        try
        {
            var value = JsonSerializer.Deserialize<T>(json, JsonOptions);
            return value is null
                ? Result.Fail<T>(ErrorsCodes.UnExpectedError)
                : Result.Ok(value);
        }
        catch (JsonException)
        {
            return Result.Fail<T>(ErrorsCodes.UnExpectedError);
        }
    }
}

