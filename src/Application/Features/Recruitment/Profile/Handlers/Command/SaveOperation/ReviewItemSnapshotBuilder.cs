using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.SaveOperation;

internal static class ReviewItemSnapshotBuilder
{
    public static object? GetSectionSnapshot(User user, UserProfile profile, ProfileSection section)
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
                FullNameAr = profile.User?.FullNameAr,
                FullNameEn = profile.User?.FullNameEn,
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
                SponsorTypeId = profile.SponsorProfile?.SponsorTypeId,
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
                Unit = profile.ResidenceAddress?.UnitNo,
                profile.User!.PhoneNumber,
                profile.User!.Email
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

    public static object? GetFieldValue(UserProfile profile, string? fieldPath)
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

    public static object? GetRowSnapshot(UserProfile profile, ProfileSection section, Guid? entityId, ReviewItem item)
    {
        if (entityId is null || entityId == Guid.Empty)
            return null;

        return section switch
        {
            ProfileSection.Qualifications => SnapshotRow(profile.Qualifications, entityId, Snapshot),
            ProfileSection.Experience => SnapshotRow(profile.Experiences, entityId, Snapshot),
            ProfileSection.TrainingCourses => SnapshotRow(profile.TrainingCourses, entityId, Snapshot),
            ProfileSection.CertificatesAndAwards => SnapshotRow(profile.Achievements, entityId, Snapshot),
            ProfileSection.Skills => SnapshotRow(profile.Skills, entityId, skill => new { skill.SkillId, skill.LevelId }),
            ProfileSection.Languages => SnapshotRow(profile.Languages, entityId, language => new
            {
                language.LanguageId,
                language.SpeakingLevelId,
                language.WritingLevelId,
                language.ReadingLevelId
            }),
            ProfileSection.Attachments => SnapshotRow(profile.AdditionalAttachments, entityId, attachment => new
            {
                AttachmentResourceId = attachment.AttachmentId,
                Title = item.AttachmentTitle,
                attachment.FileName
            }),
            _ => null
        };
    }

    public static object? GetAttachmentSnapshot(UserProfile profile, ReviewItem item)
    {
        var resourceId = GetAttachmentResourceId(profile, item);
        return resourceId is null ? null : new { resourceId };
    }

    public static Guid? GetAttachmentResourceId(UserProfile profile, ReviewItem item)
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
        q.RatingId,
        q.CertificateId
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
        e.QualificationId,
        e.CertificateId
    };

    private static object Snapshot(TrainingCourse t) => new
    {
        t.Title,
        t.Provider,
        t.CountryId,
        t.StartDate,
        t.EndDate,
        t.Description,
        t.SpecializationRelation,
        t.CertificateId
    };

    private static object Snapshot(Achievement a) => new
    {
        a.AchievementTypeId,
        a.Title,
        a.IssuingAuthority,
        a.CountryId,
        a.IssueDate,
        a.Description,
        a.RelatedToSpecialization,
        a.AttachmentId
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
