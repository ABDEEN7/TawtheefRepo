using System.Text.Json;
using System.Text.Json.Serialization;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
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
        var submissionRepo = uow.GetEntityRepository<ProfileSubmission>();

        var profile = await profileRepo.DbSet
            .Include(p => p.User)
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

        var lastVersion = await submissionRepo.DbSet
            .Where(s => s.UserProfileId == profile.Id)
            .OrderByDescending(s => s.Version)
            .Select(s => s.Version)
            .FirstOrDefaultAsync(ct);

        var snapshot = new
        {
            Profile = profile,
            Qualifications   = profile.Qualifications,
            Experiences      = profile.Experiences,
            TrainingCourses  = profile.TrainingCourses,
            Achievements     = profile.Achievements,
            Skills           = profile.Skills,
            Languages        = profile.Languages,
            Attachments      = profile.AdditionalAttachments,
            ResidenceAddress = profile.ResidenceAddress,
            SponsorProfile   = profile.SponsorProfile
        };

        var json = JsonSerializer.Serialize(snapshot,
            new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles });

        var submission = new ProfileSubmission
        {
            UserProfileId = profile.Id,
            Version       = lastVersion + 1,
            SubmittedAtUtc = time.GetUtcNow().UtcDateTime,
            SnapshotJson  = json
        };

        await submissionRepo.AddAsync(submission);
        if (profile.User is not null)
        {
            profile.User.Status = UserStatus.Submitted;
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
