using Application.Recruitment.Features.Profile.Command;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command;

public sealed class ResubmitUserProfileHandler(IUnitOfWork uow)
    : ICommandHandler<ResubmitUserProfileCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ResubmitUserProfileCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status != UserProfileStatus.RequiresUpdate)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        if (!profile.IsCompleted())
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotCompleted);

        var reviewRepo      = uow.GetEntityRepository<ReviewItem>();
        var assignmentRepo  = uow.GetEntityRepository<ProfileAssignment>();
        var loggerRepo      = uow.GetEntityRepository<UserProfileLogger>();

        // -----------------------------------------
        // 1) Deactivate assignments (same behavior)
        // -----------------------------------------
        var activeAssignments = await assignmentRepo.DbSet
            .Where(a => a.UserProfileId == profile.Id && a.IsActive)
            .ToListAsync(ct);

        foreach (var assignment in activeAssignments)
        {
            assignment.Deactivate();

            await loggerRepo.AddAsync(new UserProfileLogger
            {
                UserProfileId  = profile.Id,
                PerformedById  = cmd.UserId,
                ActionType     = UserProfileLogConstants.ActionTypes.ProfileUnassigned,
                Notes          = UserProfileLogConstants.Notes.ProfileResubmittedToDistribution,
                Section        = UserProfileLogConstants.Sections.Assignment,
                EntityId       = assignment.Id
            }, ct);
        }

        // ---------------------------------------------------------
        // 2) RESUBMIT RULE: DO NOT CREATE new review items
        //    Only reopen "Solved" items to "Pending" when changed
        // ---------------------------------------------------------
        var items = await reviewRepo.DbSet
            .Where(x => x.UserProfileId == profile.Id)
            .ToListAsync(ct);

        foreach (var item in items)
        {
            // Backfill empty hashes (your bug: section hashes were stored empty)
            var previousHash = item.CurrentHash;

            var currentValue = GetCurrentValue(profile, item);
            item.UpdateHash(currentValue);

            var valueChanged = !string.Equals(previousHash, item.CurrentHash, StringComparison.Ordinal);

            if (!valueChanged)
                continue;

            // Required: only convert Solved -> Pending (no new items)
            if (item.Status == ReviewStatus.Solved)
            {
                Reopen(item);
            }
        }

        // ---------------------------------------------------------
        // 3) Submit again
        // ---------------------------------------------------------
        profile.Status = UserProfileStatus.Submitted;
        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }

    private static void Reopen(ReviewItem item)
    {
        item.Status = ReviewStatus.Pending;
        item.IsOutdated = true;

        // Clear review metadata because we reopened
        item.ReviewedAtUtc = null;
        item.ReviewedById  = null;
        item.ReviewerNote  = null;
    }

    // -----------------------
    // Resolve current value (same logic as ReviewItemSaveHelper)
    // -----------------------
    private static object? GetCurrentValue(UserProfile profile, ReviewItem item)
    {
        return item.TargetType switch
        {
            ReviewTargetType.Section     => GetSectionSnapshot(profile, item.Section),
            ReviewTargetType.Field       => GetFieldValue(profile, item.FieldPath),
            ReviewTargetType.Row         => GetRowSnapshot(profile, item.Section, item.EntityId, item),
            ReviewTargetType.Attachment  => GetAttachmentSnapshot(profile, item),
            _ => null
        };
    }

    private static object? GetSectionSnapshot(UserProfile profile, ProfileSection section)
    {
        return section switch
        {
            ProfileSection.Prerequisites => new
            {
                profile.CandidateTypeId,
                profile.TargetEntityId,
                profile.OfficeId,
                QidExpiry = profile.QIDExpiry,
                profile.ResumeAttachmentId,
                profile.NationalCardId
            },
            ProfileSection.Personal => new
            {
                profile.User?.FullNameAr,
                profile.User?.FullNameEn,
                profile.NationalNumber,
                QidExpiry = profile.QIDExpiry,
                profile.BirthDate,
                profile.NationalityId,
                profile.GenderId,
                profile.ReligionId,
                profile.MaritalStatusId,
                profile.ChildrenCount,
                profile.HasDisability,
                profile.DisabilityDetails,
                profile.SponsorProfile?.SponsorTypeId,
                SponsorEmployerName = profile.SponsorProfile?.SponsorName,
                SponsorEmployerNumber = profile.SponsorProfile?.SponsorNumber,
                SponsorQidExpiry = profile.SponsorProfile?.QIDExpiry
            },
            ProfileSection.Contact => new
            {
                profile.ResidenceCountryId,
                profile.InterviewLocationId,
                profile.Address,
                Zone = profile.ResidenceAddress?.ZoneNo,
                Street = profile.ResidenceAddress?.StreetNo,
                Building = profile.ResidenceAddress?.BuildingNo,
                Unit = profile.ResidenceAddress?.UnitNo
            },
            ProfileSection.Qualifications => profile.Qualifications?
                .OrderBy(q => q.Id)
                .Select(Snapshot)
                .ToList(),
            ProfileSection.Experience => profile.Experiences?
                .OrderBy(e => e.Id)
                .Select(Snapshot)
                .ToList(),
            ProfileSection.TrainingCourses => profile.TrainingCourses?
                .OrderBy(t => t.Id)
                .Select(Snapshot)
                .ToList(),
            ProfileSection.CertificatesAndAwards => profile.Achievements?
                .OrderBy(a => a.Id)
                .Select(Snapshot)
                .ToList(),
            ProfileSection.Skills => profile.Skills?
                .OrderBy(s => s.Id)
                .Select(skill => new { skill.SkillId, skill.LevelId })
                .ToList(),
            ProfileSection.Languages => profile.Languages?
                .OrderBy(l => l.Id)
                .Select(language => new
                {
                    language.LanguageId,
                    language.SpeakingLevelId,
                    language.WritingLevelId,
                    language.ReadingLevelId
                })
                .ToList(),
            ProfileSection.Attachments => profile.AdditionalAttachments?
                .OrderBy(a => a.Id)
                .Select(attachment => new { AttachmentResourceId = attachment.AttachmentId, attachment.FileName })
                .ToList(),
            _ => null
        };
    }

    private static object? GetFieldValue(UserProfile profile, string? fieldPath)
    {
        if (string.IsNullOrWhiteSpace(fieldPath))
            return null;

        if (TryResolveFieldPath(profile, fieldPath, out var value))
            return value;

        return profile.User is not null && TryResolveFieldPath(profile.User, fieldPath, out value)
            ? value
            : null;
    }

    private static bool TryResolveFieldPath(object source, string fieldPath, out object? value)
    {
        value = source;

        foreach (var segment in fieldPath.Split('.', StringSplitOptions.RemoveEmptyEntries))
        {
            if (value is null)
                return true;

            var property = value.GetType().GetProperty(segment);
            if (property is null)
            {
                value = null;
                return false;
            }

            value = property.GetValue(value);
        }

        return true;
    }

    private static object? GetRowSnapshot(UserProfile profile, ProfileSection section, Guid? entityId, ReviewItem item)
    {
        if (entityId is null || entityId == Guid.Empty)
            return null;

        return section switch
        {
            ProfileSection.Qualifications => SnapshotRow(profile.Qualifications, entityId, Snapshot),
            ProfileSection.Experience => SnapshotRow(profile.Experiences, entityId, Snapshot),
            ProfileSection.TrainingCourses => SnapshotRow(profile.TrainingCourses, entityId, Snapshot),
            ProfileSection.CertificatesAndAwards => SnapshotRow(profile.Achievements, entityId, Snapshot),
            ProfileSection.Skills => SnapshotRow(profile.Skills, entityId, s => new { s.SkillId, s.LevelId }),
            ProfileSection.Languages => SnapshotRow(profile.Languages, entityId, l => new
            {
                l.LanguageId,
                l.SpeakingLevelId,
                l.WritingLevelId,
                l.ReadingLevelId
            }),
            ProfileSection.Attachments => SnapshotRow(profile.AdditionalAttachments, entityId, a => new
            {
                AttachmentResourceId = a.AttachmentId,
                Title = item.AttachmentTitle,
                a.FileName
            }),
            _ => null
        };
    }

    private static object? GetAttachmentSnapshot(UserProfile profile, ReviewItem item)
    {
        var resourceId = GetAttachmentResourceId(profile, item);
        return resourceId is null ? null : new { resourceId };
    }

    private static Guid? GetAttachmentResourceId(UserProfile profile, ReviewItem item)
    {
        if (item.FieldPath == nameof(UserProfile.ResumeAttachmentId))
            return profile.ResumeAttachmentId;

        if (item.FieldPath == nameof(UserProfile.NationalCardId))
            return profile.NationalCardId;

        if (item.FieldPath == nameof(UserProfile.BirthdayCertificateId))
            return profile.BirthdayCertificateId;

        if (item.FieldPath == nameof(UserProfile.MarriageCertificateId))
            return profile.MarriageCertificateId;

        if (item.FieldPath == ProfileReviewConstants.FieldPaths.SponsorCardResourceId)
            return profile.SponsorProfile?.SponsorCardId;

        if (item.FieldPath == ProfileReviewConstants.FieldPaths.NationalAddressCertificateId)
            return profile.ResidenceAddress?.CertificateId;

        if (item.FieldPath == ProfileReviewConstants.FieldPaths.AdditionalAttachments ||
            item.EntityName == ProfileReviewConstants.EntityNames.ProfileAdditionalAttachment)
        {
            return profile.AdditionalAttachments?
                .FirstOrDefault(a => a.Id == item.EntityId)
                ?.AttachmentId;
        }

        return item.EntityName == ProfileReviewConstants.EntityNames.Attachment
            ? profile.AdditionalAttachments?.FirstOrDefault(a => a.Id == item.EntityId)?.AttachmentId
            : item.ResourceId;
    }

    private static object Snapshot(Qualification q) => new
    {
        q.DegreeId,
        q.CountryId,
        q.MajorId,
        q.SubMajorId,
        q.UniversityId,
        q.GraduationYear,
        q.StudyTypeId,
        q.GPA,
        q.RatingId
    };

    private static object Snapshot(Experience e) => new
    {
        e.EmployerName,
        e.JobTitle,
        e.CountryId,
        e.StartDate,
        e.EndDate,
        e.Description,
        e.SpecializationRelation,
        e.QualificationId
    };

    private static object Snapshot(TrainingCourse t) => new
    {
        t.Title,
        t.Provider,
        t.CountryId,
        t.StartDate,
        t.EndDate,
        t.Description,
        t.SpecializationRelation
    };

    private static object Snapshot(Achievement a) => new
    {
        a.AchievementTypeId,
        a.Title,
        a.IssuingAuthority,
        a.CountryId,
        a.IssueDate,
        a.Description,
        a.RelatedToSpecialization
    };

    private static object? SnapshotRow<T>(
        IEnumerable<T>? items,
        Guid? entityId,
        Func<T, object> snapshot)
        where T : EventEntity
    {
        if (items is null)
            return null;

        var item = items.FirstOrDefault(entry => entry.Id == entityId);
        return item is null ? null : snapshot(item);
    }
}
