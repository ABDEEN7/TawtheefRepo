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
            .Include(p => p.Qualifications)
            .Include(p => p.Experiences)
            .Include(p => p.TrainingCourses)
            .Include(p => p.Achievements)
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
        var now = DateTime.UtcNow;

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
        
        profile.Status = UserProfileStatus.Submitted;
        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
