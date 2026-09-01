using Application.Recruitment.Features.Profile.Command;
using Application.Recruitment.Features.Profile.DTOs;
using Application.Recruitment.Features.Profile.Handlers.Command.SaveOperation;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command;

public sealed class SubmitUserProfileHandler(IUnitOfWork uow, UserManager<User> userManager)
    : IRequestHandler<SubmitUserProfileCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SubmitUserProfileCommand cmd, CancellationToken ct)
    {
        var user = await userManager.Users
            .OfType<ApplicantUser>()
            .FirstOrDefaultAsync(u => u.Id == cmd.UserId && !u.IsDeleted, ct);
        if (user is null) return Result.Fail<Unit>(ErrorsCodes.UserNotFound);

        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status != UserProfileStatus.InCreation)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        if (!profile.IsCompleted())
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotCompleted);

        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();
        var loggerRepo = uow.GetEntityRepository<UserProfileLogger>();

        var activeAssignments = await assignmentRepo.DbSet
            .Where(a => a.UserProfileId == profile.Id && a.IsActive)
            .ToListAsync(ct);

        foreach (var assignment in activeAssignments)
        {
            assignment.Deactivate();
            var unassignNote = JsonSerializer.Serialize(new
            {
                eventType = "AssignmentReassigned",
                newAssignedUserId = (Guid?)null,
                newAssignedUserName = (string?)null,
                message = UserProfileLogConstants.Notes.ProfileResubmittedToDistribution
            });

            await loggerRepo.AddAsync(
                new UserProfileLogger
                {
                    UserProfileId = profile.Id,
                    PerformedById = cmd.UserId,
                    ActionType = UserProfileLogConstants.ActionTypes.ProfileUnassigned,
                    Notes = unassignNote,
                    Section = UserProfileLogConstants.Sections.Assignment,
                    EntityId = assignment.Id
                }, ct);
        }

        foreach (var sec in ProfileApprovalFlow.Sections)
        {
            var snapshot = ReviewItemSnapshotBuilder.GetSectionSnapshot(user, profile, sec); // shared helper
            await reviewRepo.AddAsync(NewSectionReviewItem(profile, sec, snapshot), ct);
        }

        foreach (var item in BuildMixedSectionDataItems(profile))
            await reviewRepo.AddAsync(item, ct);

        foreach (var item in BuildProfileFiles(profile))
            await reviewRepo.AddAsync(item, ct);

        await AddRowsAsync(reviewRepo, profile);

        profile.Status = UserProfileStatus.Submitted;
        user.IsCompletedProfile = true;

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return Result.Fail<Unit>(ErrorsCodes.UserUpdateFailed);
        }

        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }

    private static IEnumerable<ReviewItem> BuildMixedSectionDataItems(UserProfile profile)
    {
        foreach (var section in new[] { ProfileSection.Prerequisites, ProfileSection.Personal, ProfileSection.Contact })
        {
            var item = ReviewItem.Create(
                profile.Id,
                section,
                ReviewTargetType.Field,
                ProfileReviewConstants.FieldPaths.SectionData,
                currentValue: ReviewItemSnapshotBuilder.GetSectionDataSnapshot(profile, section));

            Normalize(item);
            yield return item;
        }
    }

    // -----------------------
    // Section
    // -----------------------
    private static ReviewItem NewSectionReviewItem(UserProfile profile, ProfileSection sec, object? snapshot)
    {
        var item = ReviewItem.Create(profile.Id, sec, ReviewTargetType.Section, currentValue: snapshot);

        if (ShouldAutoApproveEmptySection(profile, sec))
        {
            item.Status = ReviewStatus.Approved;
            item.ApprovedHash = item.CurrentHash;
            item.IsOutdated = false;
            item.ReviewedAtUtc = null;
            item.ReviewedById = null;
            item.ReviewerNote = null;
            return item;
        }

        Normalize(item);
        return item;
    }

    private static bool ShouldAutoApproveEmptySection(UserProfile profile, ProfileSection section)
    {
        return section switch
        {
            ProfileSection.Qualifications => profile.Qualifications is null || profile.Qualifications.Count == 0,
            ProfileSection.Experience => profile.Experiences is null || profile.Experiences.Count == 0,
            ProfileSection.TrainingCourses => profile.TrainingCourses is null || profile.TrainingCourses.Count == 0,
            ProfileSection.CertificatesAndAwards => profile.Achievements is null || profile.Achievements.Count == 0,
            ProfileSection.Skills => profile.Skills is null || profile.Skills.Count == 0,
            ProfileSection.Languages => profile.Languages is null || profile.Languages.Count == 0,
            ProfileSection.Attachments => profile.AdditionalAttachments is null ||
                                          profile.AdditionalAttachments.Count == 0,
            _ => false
        };
    }

    // -----------------------
    // Profile-level files
    // -----------------------
    private static IEnumerable<ReviewItem> BuildProfileFiles(UserProfile profile)
    {
        if (profile.BirthdayCertificateId is not null)
            yield return NewFile(
                profile.Id,
                ProfileSection.Prerequisites,
                nameof(profile.BirthdayCertificateId),
                profile.BirthdayCertificateId.Value,
                ProfileReviewConstants.AttachmentTitles.BirthCertificate);

        if (profile.MarriageCertificateId is not null)
            yield return NewFile(
                profile.Id,
                ProfileSection.Prerequisites,
                nameof(profile.MarriageCertificateId),
                profile.MarriageCertificateId.Value,
                ProfileReviewConstants.AttachmentTitles.MarriageCertificate);

        if (profile.ResumeAttachmentId is not null)
            yield return NewFile(
                profile.Id,
                ProfileSection.Prerequisites,
                nameof(profile.ResumeAttachmentId),
                profile.ResumeAttachmentId.Value,
                ProfileReviewConstants.AttachmentTitles.Resume);

        if (profile.NationalCardId is not null)
            yield return NewFile(
                profile.Id,
                ProfileSection.Prerequisites,
                nameof(profile.NationalCardId),
                profile.NationalCardId.Value,
                ProfileReviewConstants.AttachmentTitles.NationalCard);

        if (profile.SponsorProfile?.SponsorCardId is not null)
            yield return NewFile(
                profile.Id,
                ProfileSection.Personal,
                ProfileReviewConstants.FieldPaths.SponsorCardResourceId,
                profile.SponsorProfile.SponsorCardId.Value,
                ProfileReviewConstants.AttachmentTitles.SponsorCard,
                nameof(profile.SponsorProfile),
                profile.SponsorProfile.Id);

        if (profile.ResidenceAddress?.CertificateId is not null)
            yield return NewFile(
                profile.Id,
                ProfileSection.Contact,
                ProfileReviewConstants.FieldPaths.NationalAddressCertificateId,
                profile.ResidenceAddress.CertificateId,
                ProfileReviewConstants.AttachmentTitles.NationalAddressCertificate,
                nameof(profile.ResidenceAddress),
                profile.ResidenceAddress.Id);

        if (profile.AdditionalAttachments is not null)
        {
            foreach (var a in profile.AdditionalAttachments)
            {
                yield return NewFile(
                    profile.Id,
                    ProfileSection.Attachments,
                    ProfileReviewConstants.FieldPaths.AdditionalAttachments,
                    a.AttachmentId,
                    a.FileName,
                    entityName: ProfileReviewConstants.EntityNames.ProfileAdditionalAttachment,
                    entityId: a.Id
                );
            }
        }
    }

    private static ReviewItem NewFile(
        Guid profileId,
        ProfileSection section,
        string fieldPath,
        Guid resourceId,
        string title,
        string? entityName = null,
        Guid? entityId = null)
    {
        var item = ReviewItem.Create(
            profileId,
            section,
            ReviewTargetType.Attachment,
            fieldPath,
            entityName,
            entityId,
            resourceId,
            new { resourceId });

        item.AttachmentTitle = title;
        Normalize(item);
        return item;
    }

    // -----------------------
    // Rows ONLY
    // -----------------------
    private static async Task AddRowsAsync(IGenericRepository<ReviewItem> repo, UserProfile profile)
    {
        if (profile.Qualifications is not null)
            foreach (var q in profile.Qualifications)
                await repo.AddAsync(NewRow(profile.Id, ProfileSection.Qualifications,
                    ProfileReviewConstants.EntityNames.Qualification, q.Id,
                    ReviewItemSnapshotBuilder.GetRowSnapshot(profile, ProfileSection.Qualifications, q.Id,
                        new ReviewItem())));

        if (profile.Experiences is not null)
            foreach (var e in profile.Experiences)
                await repo.AddAsync(NewRow(profile.Id, ProfileSection.Experience,
                    ProfileReviewConstants.EntityNames.Experience, e.Id,
                    ReviewItemSnapshotBuilder.GetRowSnapshot(profile, ProfileSection.Experience, e.Id,
                        new ReviewItem())));

        if (profile.TrainingCourses is not null)
            foreach (var t in profile.TrainingCourses)
                await repo.AddAsync(NewRow(profile.Id, ProfileSection.TrainingCourses,
                    ProfileReviewConstants.EntityNames.TrainingCourse, t.Id,
                    ReviewItemSnapshotBuilder.GetRowSnapshot(profile, ProfileSection.TrainingCourses, t.Id,
                        new ReviewItem())));

        if (profile.Achievements is not null)
            foreach (var a in profile.Achievements)
                await repo.AddAsync(NewRow(profile.Id, ProfileSection.CertificatesAndAwards,
                    ProfileReviewConstants.EntityNames.Achievement, a.Id,
                    ReviewItemSnapshotBuilder.GetRowSnapshot(profile, ProfileSection.CertificatesAndAwards, a.Id,
                        new ReviewItem())));
    }

    private static ReviewItem NewRow(
        Guid profileId,
        ProfileSection section,
        string entityName,
        Guid entityId,
        object? snapshot)
    {
        var item = ReviewItem.Create(
            profileId,
            section,
            ReviewTargetType.Row,
            null,
            entityName,
            entityId,
            null,
            snapshot);

        Normalize(item);
        return item;
    }

    private static void Normalize(ReviewItem item)
    {
        item.Status = ReviewStatus.Pending;
        item.IsOutdated = true;
        item.ReviewedAtUtc = null;
        item.ReviewedById = null;
        item.ReviewerNote = null;
    }
}
