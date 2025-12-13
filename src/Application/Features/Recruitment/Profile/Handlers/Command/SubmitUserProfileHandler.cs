using System.Text.Json;
using System.Text.Json.Serialization;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.DTOs;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command;


public sealed class SubmitUserProfileHandler(
    IUnitOfWork uow,
    TimeProvider time
) : IRequestHandler<SubmitUserProfileCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SubmitUserProfileCommand cmd, CancellationToken ct)
    {
        var profileRepo  = uow.GetEntityRepository<UserProfile>();
        var profile = await profileRepo.DbSet
            .Include(p => p.User)
            .Include(p => p.SponsorProfile)
            .Include(p => p.ResidenceAddress)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.Major)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.University)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.Certificate)
            .Include(p => p.Experiences)!.ThenInclude(e => e.Certificate)
            .Include(p => p.TrainingCourses)!.ThenInclude(t => t.Certificate)
            .Include(p => p.Achievements)!.ThenInclude(a => a.AchievementType)
            .Include(p => p.Achievements)!.ThenInclude(a => a.Attachment)
            .Include(p => p.Skills)
            .Include(p => p.Languages)
            .FirstOrDefaultAsync(p => p.UserId == cmd.UserId, ct);

        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);
        
        if(!profile.IsCompleted())
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotCompleted);

        var submissionRepo = uow.GetEntityRepository<ProfileSubmission>();
        var lastVersion = await submissionRepo.DbSet
            .Where(s => s.UserProfileId == profile.Id)
            .OrderByDescending(s => s.Version)
            .Select(s => s.Version)
            .FirstOrDefaultAsync(ct);

        var snapshot = new
        {
            Profile = profile,
            profile.Qualifications,
            profile.Experiences,
            profile.TrainingCourses,
            profile.Achievements,
            profile.Skills,
            profile.Languages,
            Attachments      = profile.AdditionalAttachments,
            profile.ResidenceAddress,
            profile.SponsorProfile
        };

        var json = JsonSerializer.Serialize(snapshot,
            new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles });

        var newVersion = lastVersion + 1;
        var submission = new ProfileSubmission
        {
            UserProfileId = profile.Id,
            Version       = lastVersion + 1,
            SubmittedAtUtc = time.GetUtcNow().UtcDateTime,
            SnapshotJson  = json
        };

        await submissionRepo.AddAsync(submission);

        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        foreach (var sec in ProfileApprovalFlow.Sections)
        {
            var item = ReviewItem.Create(
                userProfileId: profile.Id,
                section: sec,
                targetType: ReviewTargetType.Section,
                fieldPath: null,
                entityName: null,
                entityId: null,
                resourceId: null,
                currentValue: null
            );

            item.Version = newVersion;
            item.Status = ReviewStatus.Pending;
            item.IsOutdated = true;
            item.ReviewedAtUtc = null;
            item.ReviewedById = null;
            item.ReviewerNote = null;

            await reviewRepo.AddAsync(item);
        }

        foreach (var qualification in profile.Qualifications ?? [])
        {
            if (qualification.CertificateId is null) continue;

            await AddAttachmentItem(
                ProfileSection.Qualifications,
                nameof(Qualification),
                qualification.Id,
                qualification.CertificateId.Value,
                qualification.Certificate?.FileName
                    ?? qualification.Major?.Name
                    ?? "Qualification file");
        }

        foreach (var experience in profile.Experiences ?? [])
        {
            if (experience.CertificateId == Guid.Empty) continue;

            await AddAttachmentItem(
                ProfileSection.Experience,
                nameof(Experience),
                experience.Id,
                experience.CertificateId,
                experience.Certificate?.FileName
                    ?? experience.JobTitle
                    ?? "Experience certificate");
        }

        foreach (var training in profile.TrainingCourses ?? [])
        {
            if (training.CertificateId == Guid.Empty) continue;

            await AddAttachmentItem(
                ProfileSection.TrainingCourses,
                nameof(TrainingCourse),
                training.Id,
                training.CertificateId,
                training.Certificate?.FileName
                    ?? training.Title
                    ?? "Training certificate");
        }

        foreach (var achievement in profile.Achievements ?? [])
        {
            if (achievement.AttachmentId == Guid.Empty) continue;

            await AddAttachmentItem(
                ProfileSection.CertificatesAndAwards,
                nameof(Achievement),
                achievement.Id,
                achievement.AttachmentId,
                achievement.Attachment?.FileName
                    ?? achievement.Title
                    ?? "Certificate");
        }

        profile.Status = UserProfileStatus.Submitted;
        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);

        async Task AddAttachmentItem(
            ProfileSection section,
            string entityName,
            Guid entityId,
            Guid resourceId,
            string attachmentTitle)
        {
            var item = ReviewItem.Create(
                userProfileId: profile.Id,
                section: section,
                targetType: ReviewTargetType.Attachment,
                fieldPath: null,
                entityName: entityName,
                entityId: entityId,
                resourceId: resourceId,
                currentValue: resourceId);

            item.AttachmentTitle = attachmentTitle;
            item.Version = newVersion;
            item.Status = ReviewStatus.Pending;
            item.IsOutdated = true;
            item.ReviewedAtUtc = null;
            item.ReviewedById = null;
            item.ReviewerNote = null;

            await reviewRepo.AddAsync(item);
        }
    }
}
