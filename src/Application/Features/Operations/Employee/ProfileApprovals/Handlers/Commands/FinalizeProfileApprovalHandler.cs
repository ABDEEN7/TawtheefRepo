using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.Handlers.Commands;

public sealed class FinalizeProfileApprovalHandler(
    IUnitOfWork uow,
    IFileStorageService storage)
    : IRequestHandler<FinalizeProfileApprovalCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(FinalizeProfileApprovalCommand cmd, CancellationToken ct)
    {
        if (cmd.OfficerId == Guid.Empty)
            return Result.Fail<Unit>(ErrorsCodes.InvalidUserIdentifier);

        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        var decisionRepo = uow.GetEntityRepository<ProfileReviewDecision>();
        var auditRepo = uow.GetEntityRepository<AuditTrailEntry>();

        var profile = await profileRepo.DbSet
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == cmd.UserProfileId, ct);

        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        var isAssigned = await assignmentRepo.DbSet
            .AnyAsync(a => a.UserProfileId == profile.Id && a.EmployeeId == cmd.OfficerId && a.IsActive, ct);

        if (!isAssigned && !cmd.HasManagerOverride)
            return Result.Fail<Unit>(ErrorsCodes.UnauthorizedAction);

        var reviewItems = await reviewRepo.DbSet
            .Where(r => r.UserProfileId == profile.Id)
            .ToListAsync(ct);

        var decision = new ProfileReviewDecision
        {
            UserProfileId = profile.Id,
            Notes = cmd.Notes,
            Summary = cmd.Summary,
            Action = cmd.Action
        };

        switch (cmd.Action)
        {
            case FinalApprovalAction.ApproveProfile:
                if (!profile.IsCompleted())
                    return Result.Fail<Unit>(ErrorsCodes.MandatoryFieldsIncomplete);

                if (reviewItems.Any(r => r.Status != ReviewStatus.Approved))
                    return Result.Fail<Unit>(ErrorsCodes.UnapprovedItemsExist);

                profile.Status = UserProfileStatus.Approved;
                break;

            case FinalApprovalAction.NeedsCorrection:
                if (string.IsNullOrWhiteSpace(cmd.Notes))
                    return Result.Fail<Unit>(ErrorsCodes.NotesRequiredForCorrection);
                if (cmd.NeedsCorrectionItems.Count == 0)
                    return Result.Fail<Unit>(ErrorsCodes.NeedsCorrectionTargetsRequired);

                var targets = reviewItems.Where(r => cmd.NeedsCorrectionItems.Contains(r.Id)).ToList();
                if (targets.Count == 0)
                    return Result.Fail<Unit>(ErrorsCodes.ReviewItemNotFound);

                foreach (var target in targets)
                {
                    target.RequestChanges(cmd.OfficerId, cmd.Notes!);
                }

                profile.Status = UserProfileStatus.RequiresUpdate;
                break;

            case FinalApprovalAction.RejectProfile:
                if (cmd.RejectionDocument is null)
                    return Result.Fail<Unit>(ErrorsCodes.RejectionDocumentRequired);
                if (string.IsNullOrWhiteSpace(cmd.Notes))
                    return Result.Fail<Unit>(ErrorsCodes.NotesRequiredForCorrection);

                var rejectionResourceId = await SaveFileAsync(cmd.RejectionDocument, profile.Id, "rejection", ct);
                if (rejectionResourceId is null)
                    return Result.Fail<Unit>(ErrorsCodes.RejectionDocumentRequired);

                decision.AttachmentResourceId = rejectionResourceId;
                profile.Status = UserProfileStatus.AdminCancelled;
                break;

            case FinalApprovalAction.BlockProfile:
                if (string.IsNullOrWhiteSpace(cmd.Notes))
                    return Result.Fail<Unit>(ErrorsCodes.NotesRequiredForCorrection);

                if (profile.User is not null)
                    profile.User.IsBlocked = true;

                profile.Status = UserProfileStatus.AdminCancelled;
                break;

            case FinalApprovalAction.ExceptionalApproval:
                if (cmd.ExceptionalFile is null)
                    return Result.Fail<Unit>(ErrorsCodes.ExceptionalFileRequired);

                var exceptionalResourceId = await SaveFileAsync(cmd.ExceptionalFile, profile.Id, "exception", ct);
                if (exceptionalResourceId is null)
                    return Result.Fail<Unit>(ErrorsCodes.ExceptionalFileRequired);

                decision.AttachmentResourceId = exceptionalResourceId;
                decision.ExceptionalFlag = true;
                profile.Status = UserProfileStatus.Approved;
                break;

            default:
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);
        }

        await decisionRepo.AddAsync(decision);

        await auditRepo.AddAsync(new AuditTrailEntry
        {
            UserProfileId = profile.Id,
            UserId = cmd.OfficerId,
            ActionType = cmd.Action.ToString(),
            Notes = cmd.Notes,
            AttachmentId = decision.AttachmentResourceId
        });

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }

    private async Task<Guid?> SaveFileAsync(IFormFile file, Guid profileId, string prefix, CancellationToken ct)
    {
        var extension = Path.GetExtension(file.FileName);
        var blobKey = $"profiles/{profileId}/{prefix}-{Guid.NewGuid()}{extension}";

        await using var uploadStream = file.OpenReadStream();
        var saveResult = await storage.SaveAsync(uploadStream, blobKey, ct);
        if (saveResult.IsFailed) return null;

        var resource = new Resource
        {
            Name = file.FileName,
            Url = saveResult.Value.BlobKey,
            Key = Guid.NewGuid().ToString(),
            Type = file.ContentType,
            Size = saveResult.Value.Size
        };

        var repo = uow.GetEntityRepository<Resource>();
        var addResult = await repo.AddAsync(resource);
        if (addResult.IsFailed) return null;

        return resource.Id;
    }
}
