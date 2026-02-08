using Application.Recruitment.Features.Profile.Command;
using Application.Recruitment.Features.Profile.DTOs;
using Application.Recruitment.Features.Profile.Handlers.Command.SaveOperation;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command;

public sealed class SubmitUserProfileHandler(IUnitOfWork uow, UserManager<User> userManager)
    : ICommandHandler<SubmitUserProfileCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SubmitUserProfileCommand cmd, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(cmd.UserId.ToString());
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

            await loggerRepo.AddAsync(new UserProfileLogger
            {
                UserProfileId = profile.Id,
                PerformedById = cmd.UserId,
                ActionType = UserProfileLogConstants.ActionTypes.ProfileUnassigned,
                Notes = UserProfileLogConstants.Notes.ProfileResubmittedToDistribution,
                Section = UserProfileLogConstants.Sections.Assignment,
                EntityId = assignment.Id
            });
        }

        // 1️⃣ Section-level review items
        foreach (var sec in ProfileApprovalFlow.Sections)
        {
            var snapshot = ReviewItemSnapshotBuilder.GetSectionSnapshot(user, profile, sec); // shared helper
            await reviewRepo.AddAsync(NewPendingSection(profile.Id, sec, snapshot));
        }

        // 2️⃣ Profile-level attachments
        foreach (var item in BuildProfileFiles(profile))
            await reviewRepo.AddAsync(item);

        // 3️⃣ Row-level entities (ONLY rows)
        AddRows(reviewRepo, profile);

        profile.Status = UserProfileStatus.Submitted;
        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }

    // -----------------------
    // Section
    // -----------------------
    private static ReviewItem NewPendingSection(Guid profileId, ProfileSection sec, object? snapshot)
    {
        var item = ReviewItem.Create(profileId, sec, ReviewTargetType.Section, currentValue: snapshot);
        Normalize(item);
        return item;
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
    private static void AddRows(IGenericRepository<ReviewItem> repo, UserProfile profile)
    {
        if (profile.Qualifications is not null)
            foreach (var q in profile.Qualifications)
                repo.AddAsync(NewRow(profile.Id, ProfileSection.Qualifications, ProfileReviewConstants.EntityNames.Qualification, q.Id, Snapshot(q)));

        if (profile.Experiences is not null)
            foreach (var e in profile.Experiences)
                repo.AddAsync(NewRow(profile.Id, ProfileSection.Experience, ProfileReviewConstants.EntityNames.Experience, e.Id, Snapshot(e)));

        if (profile.TrainingCourses is not null)
            foreach (var t in profile.TrainingCourses)
                repo.AddAsync(NewRow(profile.Id, ProfileSection.TrainingCourses, ProfileReviewConstants.EntityNames.TrainingCourse, t.Id, Snapshot(t)));

        if (profile.Achievements is not null)
            foreach (var a in profile.Achievements)
                repo.AddAsync(NewRow(profile.Id, ProfileSection.CertificatesAndAwards, ProfileReviewConstants.EntityNames.Achievement, a.Id, Snapshot(a)));
    }

    private static ReviewItem NewRow(
        Guid profileId,
        ProfileSection section,
        string entityName,
        Guid entityId,
        object snapshot)
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

    // -----------------------
    // Hash snapshots (rows)
    // -----------------------
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
}
