using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
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

        if (cmd.Status == ReviewStatus.Approved)
        {
            var hasUnapprovedChildren = await reviewRepo.DbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.UserProfileId == profile.Id &&
                    x.Section == cmd.Section &&
                    x.TargetType != ReviewTargetType.Section &&
                    !x.IsDeleted &&
                    x.Status != ReviewStatus.Approved,
                    ct);
            if (hasUnapprovedChildren)
                return Result.Fail<Unit>(ErrorsCodes.UnapprovedItemsExist);
        }

        if (cmd.Status == ReviewStatus.Approved && cmd.Section == ProfileSection.Qualifications)
        {
            var universityValidation = await QualificationUniversityReviewGuard.ValidatePersistedProfileAsync(
                uow, profile.Id, ct);
            if (universityValidation.IsFailed)
                return Result.Fail<Unit>(universityValidation.Errors);
        }
        
        item.Status = cmd.Status;
        item.ReviewerNote = cmd.Note;
        item.ReviewedById = cmd.OfficerId;
        item.ReviewedAtUtc = time.GetUtcNow().UtcDateTime;
        item.IsOutdated = false;

        var decisionNote = JsonSerializer.Serialize(new
        {
            eventType = "ReviewSectionDecision",
            section = cmd.Section.ToString(),
            status = cmd.Status.ToString(),
            reviewerNote = cmd.Note
        });

        await auditRepo.AddAsync(new AuditTrailEntry
        {
            UserProfileId = profile.Id,
            UserId = cmd.OfficerId,
            ActionType = UserProfileLogConstants.ActionTypes.ReviewSectionDecision,
            Notes = decisionNote,
            Section = cmd.Section.ToString(),
            EntityId = item.Id
        });

        await loggerRepo.AddAsync(new UserProfileLogger
        {
            UserProfileId = profile.Id,
            PerformedById = cmd.OfficerId,
            ActionType = UserProfileLogConstants.ActionTypes.ReviewSectionDecision,
            Notes = decisionNote,
            Section = cmd.Section.ToString(),
            EntityId = item.Id,
            ReviewStatus = cmd.Status
        });

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}

