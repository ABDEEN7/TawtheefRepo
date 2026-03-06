using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Handlers.Commands;

public sealed class DecideProfileSectionHandler(IUnitOfWork uow, TimeProvider time)
    : IRequestHandler<DecideProfileSectionCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(DecideProfileSectionCommand cmd, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var auditRepo = uow.GetEntityRepository<AuditTrailEntry>();
        var loggerRepo = uow.GetEntityRepository<UserProfileLogger>();

        var profile = await profileRepo.DbSet
            .FirstOrDefaultAsync(p => p.Id == cmd.UserProfileId, ct);

        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status != UserProfileStatus.UnderReview)
            return Result.Fail<Unit>(ErrorsCodes.ProfileNotUnderReview);

        if (cmd.Status != ReviewStatus.Approved && cmd.Status != ReviewStatus.NeedsCorrection)
            return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

        if (cmd.Status == ReviewStatus.NeedsCorrection && string.IsNullOrWhiteSpace(cmd.Note))
            return Result.Fail<Unit>(ErrorsCodes.NotesRequiredForCorrection);

        var reviewRepo = uow.GetEntityRepository<ReviewItem>();

        var item = await reviewRepo.DbSet
            .FirstOrDefaultAsync(x =>
                x.UserProfileId == profile.Id &&
                x.TargetType == ReviewTargetType.Section &&
                x.Section == cmd.Section, ct);
        if (item is null)
            return Result.Fail<Unit>(ErrorsCodes.ReviewItemNotFound);
        
        item.Status = cmd.Status;
        item.ReviewerNote = cmd.Note;
        item.ReviewedById = cmd.OfficerId;
        item.ReviewedAtUtc = time.GetUtcNow().UtcDateTime;
        item.IsOutdated = false;

        await auditRepo.AddAsync(new AuditTrailEntry
        {
            UserProfileId = profile.Id,
            UserId = cmd.OfficerId,
            ActionType = UserProfileLogConstants.ActionTypes.ReviewSectionDecision,
            Notes = $"Section {cmd.Section} marked {cmd.Status}",
            Section = cmd.Section.ToString(),
            EntityId = item.Id
        });

        await loggerRepo.AddAsync(new UserProfileLogger
        {
            UserProfileId = profile.Id,
            PerformedById = cmd.OfficerId,
            ActionType = UserProfileLogConstants.ActionTypes.ReviewSectionDecision,
            Notes = $"Section {cmd.Section} marked {cmd.Status}",
            Section = cmd.Section.ToString(),
            EntityId = item.Id,
            ReviewStatus = cmd.Status
        });

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}

