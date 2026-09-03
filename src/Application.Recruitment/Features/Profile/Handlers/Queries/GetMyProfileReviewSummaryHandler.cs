using Application.Recruitment.Features.Profile.DTOs;
using Application.Recruitment.Features.Profile.Queries;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Queries;


public sealed class GetMyProfileReviewSummaryHandler(
    IUnitOfWork uow,
    ILocalizationService localizationService)
    : IRequestHandler<GetMyProfileReviewSummaryQuery, IResult<MyProfileReviewSummaryDto>>
{
    public async Task<IResult<MyProfileReviewSummaryDto>> Handle(
        GetMyProfileReviewSummaryQuery request, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, request.UserId, ct: ct);

        if (profile is null)
            return Result.Fail<MyProfileReviewSummaryDto>(ErrorsCodes.ProfileNotFound);
        

        var items = await uow.GetEntityRepository<ReviewItem>()
            .DbSet.AsNoTracking()
            .Where(x =>
                x.UserProfileId == profile.Id &&
                x.ProfileChangeId == null &&
                !x.IsDeleted)
            .ToListAsync(ct);

        var activeItems = items
            .Where(item => item.IsActiveInCurrentProfile(profile))
            .ToList();

        var outstandingItems = activeItems
            .Where(item => item.IsOutstandingCandidateCorrection())
            .ToList();

        var visibleItems = activeItems
            .Where(item => item.Status != ReviewStatus.Solved)
            .ToList();

        var correctedReviewItems = activeItems
            .Where(item => item.IsCandidateCorrectedItem())
            .ToList();
        var correctedItems = correctedReviewItems
            .OrderBy(x => (int)x.Section)
            .ThenBy(x => (int)x.TargetType)
            .ThenBy(x => x.AttachmentTitle ?? x.FieldPath ?? x.EntityName ?? string.Empty)
            .Select(x => new MyProfileReviewChangedItemDto
            {
                ReviewItemId = x.Id,
                Section = x.Section,
                TargetType = x.TargetType,
                Title = ResolveDisplayTitle(profile, x),
                Note = x.ReviewerNote,
                FieldPath = x.FieldPath,
                EntityId = x.EntityId,
                EntityName = x.EntityName,
                ResourceId = x.ResourceId
            })
            .ToList();
        var displayChangedSections = correctedItems
            .Select(x => x.Section)
            .Distinct()
            .OrderBy(x => (int)x)
            .ToList();

        var lastUserChangeAt = correctedReviewItems
            .Select(i => i.UpdatedDate)
            .Where(d => d.HasValue)
            .DefaultIfEmpty()
            .Max();

        var canResubmit = profile.Status == UserProfileStatus.RequiresUpdate &&
                          correctedReviewItems.Count > 0 &&
                          !outstandingItems.Any() &&
                          profile.IsCompleted();
        
        if (profile.Status is not UserProfileStatus.InCreation && 
            profile.Status is not UserProfileStatus.RequiresUpdate &&
            profile.Status is not UserProfileStatus.Submitted)
            return Result.Ok(new MyProfileReviewSummaryDto()
            {
                UserProfileId = profile.Id,
                ProfileStatus = profile.Status,
                HasSavedChanges = correctedReviewItems.Count > 0,
                ChangedSections = displayChangedSections,
                ChangedItems = correctedItems,
                LastUserChangeAtUtc = lastUserChangeAt,
                CanResubmit = canResubmit
            });
        

        var noteItems = activeItems
            .Where(item =>
                item.IsCandidateActionableTarget() &&
                !string.IsNullOrWhiteSpace(item.ReviewerNote))
            .ToList();

        var notes = noteItems
            .Select(x => new MyProfileReviewNoteDto {
                ReviewItemId = x.Id,
                TargetType   = x.TargetType,
                Status       = x.Status,
                Title        = ResolveDisplayTitle(profile, x),
                Note         = x.ReviewerNote,
                FieldPath    = x.FieldPath,
                EntityId     = x.EntityId,
                EntityName   = x.EntityName,
                ResourceId   = x.ResourceId,
                ReviewedAtUtc = x.ReviewedAtUtc
            })
            .ToList();

        var notesBySection = notes
            .GroupBy(n => items.First(i => i.Id == n.ReviewItemId).Section)
            .ToDictionary(g => g.Key, g => g.ToList());
        
        var lastReviewerAt = visibleItems
            .Where(i => i.ReviewedAtUtc != null)
            .Select(i => i.ReviewedAtUtc)
            .DefaultIfEmpty()
            .Max();

        var sections = Enum.GetValues<ProfileSection>()
            .Select(sec => {
                var secNotes = notesBySection.TryGetValue(sec, out var list) ? list : [];

                var secPendingCount = outstandingItems.Count(i => i.Section == sec);
                var hasActionableSectionData = outstandingItems.Any(item =>
                    item.Section == sec &&
                    item.TargetType == ReviewTargetType.Field &&
                    item.FieldPath == ProfileReviewConstants.FieldPaths.SectionData);
                return new MyProfileReviewSectionDto
                {
                    Section = sec,
                    NotesCount = secNotes.Count,
                    Notes = secNotes
                        .OrderByDescending(n => n.ReviewedAtUtc)
                        .ToArray(),
                    HasActionableSectionData = hasActionableSectionData,
                    HasUserChanges = correctedReviewItems.Any(item => item.Section == sec),
                    PendingItemsCount = secPendingCount
                };
            })
            .ToArray();

        var dto = new MyProfileReviewSummaryDto {
            UserProfileId = profile.Id,
            ProfileStatus = profile.Status,
            Sections = sections,
            TotalNotes = notes.Count,
            HasSavedChanges = correctedReviewItems.Count > 0,
            ChangedSections = displayChangedSections,
            ChangedItems = correctedItems,
            LastReviewerActionAtUtc = lastReviewerAt,
            LastUserChangeAtUtc = lastUserChangeAt,
            CanResubmit = canResubmit
        };

        return Result.Ok(dto);

    }

    private string ResolveDisplayTitle(UserProfile profile, ReviewItem item)
    {
        return CleanLabel(item.AttachmentTitle) ??
               ResolveCurrentTargetTitle(profile, item) ??
               CleanLabel(item.FieldPath) ??
               CleanLabel(item.EntityName) ??
               string.Empty;
    }

    private string? ResolveCurrentTargetTitle(UserProfile profile, ReviewItem item)
    {
        if (item.TargetType == ReviewTargetType.Section)
        {
            return item.Section switch
            {
                ProfileSection.Skills => JoinLabels(profile.Skills?
                    .Select(skill => localizationService.GetLocalizedName(skill.Skill))),
                ProfileSection.Languages => JoinLabels(profile.Languages?
                    .Select(language => localizationService.GetLocalizedName(language.Language))),
                _ => null
            };
        }

        if (item.TargetType != ReviewTargetType.Row || item.EntityId is null)
            return null;

        return item.Section switch
        {
            ProfileSection.Qualifications => ResolveQualificationTitle(profile, item.EntityId.Value),
            ProfileSection.Experience => ResolveExperienceTitle(profile, item.EntityId.Value),
            ProfileSection.TrainingCourses => ResolveTrainingTitle(profile, item.EntityId.Value),
            ProfileSection.CertificatesAndAwards => ResolveAchievementTitle(profile, item.EntityId.Value),
            ProfileSection.Skills => CleanLabel(localizationService.GetLocalizedName(
                profile.Skills?.FirstOrDefault(skill => skill.Id == item.EntityId)?.Skill)),
            ProfileSection.Languages => CleanLabel(localizationService.GetLocalizedName(
                profile.Languages?.FirstOrDefault(language => language.Id == item.EntityId)?.Language)),
            ProfileSection.Attachments => CleanLabel(profile.AdditionalAttachments?
                .FirstOrDefault(attachment => attachment.Id == item.EntityId)?.FileName),
            _ => null
        };
    }

    private string? ResolveQualificationTitle(UserProfile profile, Guid entityId)
    {
        var qualification = profile.Qualifications?.FirstOrDefault(item => item.Id == entityId);
        if (qualification is null)
            return null;

        var specialization = JoinLabels([
            localizationService.GetLocalizedName(qualification.Major),
            localizationService.GetLocalizedName(qualification.SubMajor)
        ], " / ");

        return JoinLabels([
            localizationService.GetLocalizedName(qualification.Degree),
            localizationService.GetLocalizedName(qualification.University),
            specialization,
            qualification.GraduationYear?.ToString()
        ]);
    }

    private static string? ResolveExperienceTitle(UserProfile profile, Guid entityId)
    {
        var experience = profile.Experiences?.FirstOrDefault(item => item.Id == entityId);
        return experience is null
            ? null
            : JoinLabels([experience.JobTitle, experience.EmployerName]);
    }

    private static string? ResolveTrainingTitle(UserProfile profile, Guid entityId)
    {
        var training = profile.TrainingCourses?.FirstOrDefault(item => item.Id == entityId);
        return training is null
            ? null
            : JoinLabels([training.Title, training.Provider]);
    }

    private static string? ResolveAchievementTitle(UserProfile profile, Guid entityId)
    {
        var achievement = profile.Achievements?.FirstOrDefault(item => item.Id == entityId);
        return achievement is null
            ? null
            : JoinLabels([achievement.Title, achievement.IssuingAuthority]);
    }

    private static string? JoinLabels(IEnumerable<string?>? values, string separator = " - ")
    {
        if (values is null)
            return null;

        var labels = values
            .Select(CleanLabel)
            .Where(label => label is not null)
            .Cast<string>()
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        return labels.Length == 0 ? null : string.Join(separator, labels);
    }

    private static string? CleanLabel(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized) || normalized == "?" ? null : normalized;
    }
}

