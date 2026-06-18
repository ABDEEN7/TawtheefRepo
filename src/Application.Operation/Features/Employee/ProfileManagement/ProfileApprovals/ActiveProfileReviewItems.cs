using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals;

internal static class ActiveProfileReviewItems
{
    public static List<ReviewItem> ForFullReview(UserProfile profile, IEnumerable<ReviewItem> items)
    {
        var scope = ActiveProfileScope.From(profile);

        return items
            .Where(item => item.ProfileChangeId != null || IsActiveInCurrentProfile(item, scope))
            .ToList();
    }

    private static bool IsActiveInCurrentProfile(ReviewItem item, ActiveProfileScope scope)
    {
        return item.TargetType switch
        {
            ReviewTargetType.Section => true,
            ReviewTargetType.Field => true,
            ReviewTargetType.Row => IsActiveRow(item, scope),
            ReviewTargetType.Attachment => IsActiveAttachment(item, scope),
            _ => true
        };
    }

    private static bool IsActiveRow(ReviewItem item, ActiveProfileScope scope)
    {
        if (!item.EntityId.HasValue || item.EntityId.Value == Guid.Empty)
            return false;

        var entityId = item.EntityId.Value;

        return item.EntityName switch
        {
            ProfileReviewConstants.EntityNames.Qualification => scope.QualificationIds.Contains(entityId),
            ProfileReviewConstants.EntityNames.Experience => scope.ExperienceIds.Contains(entityId),
            ProfileReviewConstants.EntityNames.TrainingCourse => scope.TrainingCourseIds.Contains(entityId),
            ProfileReviewConstants.EntityNames.Achievement => scope.AchievementIds.Contains(entityId),
            ProfileReviewConstants.EntityNames.Skill => scope.SkillIds.Contains(entityId),
            ProfileReviewConstants.EntityNames.Language => scope.LanguageIds.Contains(entityId),
            ProfileReviewConstants.EntityNames.Attachment => scope.AdditionalAttachmentIds.Contains(entityId),
            ProfileReviewConstants.EntityNames.ProfileAdditionalAttachment => scope.AdditionalAttachmentIds.Contains(entityId),
            _ => true
        };
    }

    private static bool IsActiveAttachment(ReviewItem item, ActiveProfileScope scope)
    {
        return item.ResourceId.HasValue &&
               item.ResourceId.Value != Guid.Empty &&
               scope.ResourceIds.Contains(item.ResourceId.Value);
    }

    private sealed class ActiveProfileScope
    {
        public HashSet<Guid> QualificationIds { get; } = [];
        public HashSet<Guid> ExperienceIds { get; } = [];
        public HashSet<Guid> TrainingCourseIds { get; } = [];
        public HashSet<Guid> AchievementIds { get; } = [];
        public HashSet<Guid> SkillIds { get; } = [];
        public HashSet<Guid> LanguageIds { get; } = [];
        public HashSet<Guid> AdditionalAttachmentIds { get; } = [];
        public HashSet<Guid> ResourceIds { get; } = [];

        public static ActiveProfileScope From(UserProfile profile)
        {
            var scope = new ActiveProfileScope();

            scope.AddResource(profile.ResumeAttachmentId);
            scope.AddResource(profile.NationalCardId);
            scope.AddResource(profile.BirthdayCertificateId);
            scope.AddResource(profile.MarriageCertificateId);
            scope.AddResource(profile.SponsorProfile?.SponsorCardId);
            scope.AddResource(profile.ResidenceAddress?.CertificateId);

            foreach (var qualification in profile.Qualifications ?? [])
            {
                scope.QualificationIds.Add(qualification.Id);
                scope.AddResource(qualification.CertificateId);
            }

            foreach (var experience in profile.Experiences ?? [])
            {
                scope.ExperienceIds.Add(experience.Id);
                scope.AddResource(experience.CertificateId);
            }

            foreach (var trainingCourse in profile.TrainingCourses ?? [])
            {
                scope.TrainingCourseIds.Add(trainingCourse.Id);
                scope.AddResource(trainingCourse.CertificateId);
            }

            foreach (var achievement in profile.Achievements ?? [])
            {
                scope.AchievementIds.Add(achievement.Id);
                scope.AddResource(achievement.AttachmentId);
            }

            foreach (var skill in profile.Skills ?? [])
            {
                scope.SkillIds.Add(skill.Id);
            }

            foreach (var language in profile.Languages ?? [])
            {
                scope.LanguageIds.Add(language.Id);
            }

            foreach (var attachment in profile.AdditionalAttachments ?? [])
            {
                scope.AdditionalAttachmentIds.Add(attachment.Id);
                scope.AddResource(attachment.AttachmentId);
            }

            return scope;
        }

        private void AddResource(Guid? resourceId)
        {
            if (resourceId.HasValue && resourceId.Value != Guid.Empty)
                ResourceIds.Add(resourceId.Value);
        }
    }
}
