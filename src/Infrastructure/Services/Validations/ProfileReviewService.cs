using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Validations;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Infrastructure.Services.Validations;

public class ProfileReviewService(IUnitOfWork uow) : IProfileReviewService
{
    public async Task<ReviewItem> TouchSectionAsync(Guid userProfileId, ProfileSection section, Guid requestedByUserId, CancellationToken ct, object? oldValue = null, object? newValue = null)
    {
        return await TouchAsync(userProfileId, section, ReviewTargetType.Section, requestedByUserId, null, null, null, ct, null, oldValue, newValue);
    }

    public async Task<ReviewItem> TouchFieldAsync(Guid userProfileId, ProfileSection section, string fieldPath, Guid requestedByUserId, CancellationToken ct, object? oldValue = null, object? newValue = null)
    {
        return await TouchAsync(userProfileId, section, ReviewTargetType.Field, requestedByUserId, null, null, null, ct, null, oldValue, newValue, fieldPath);
    }

    public async Task<ReviewItem> TouchRowAsync(Guid userProfileId, ProfileSection section, string entityName, Guid entityId, Guid requestedByUserId, CancellationToken ct, object? oldValue = null, object? newValue = null, string? fieldPath = null)
    {
        return await TouchAsync(userProfileId, section, ReviewTargetType.Row, requestedByUserId, null, entityName, entityId, ct, null, oldValue, newValue, fieldPath);
    }

    public async Task<ReviewItem> TouchAttachmentAsync(Guid userProfileId, ProfileSection section, string? attachmentTitle, Guid resourceId, Guid requestedByUserId, CancellationToken ct, object? oldValue = null, object? newValue = null)
    {
        return await TouchAsync(userProfileId, section, ReviewTargetType.Attachment, requestedByUserId, attachmentTitle, null, null, ct, resourceId, oldValue, newValue);
    }

    private async Task<ReviewItem> TouchAsync(
        Guid userProfileId,
        ProfileSection section,
        ReviewTargetType targetType,
        Guid requestedByUserId,
        string? attachmentTitle,
        string? entityName,
        Guid? entityId,
        CancellationToken ct,
        Guid? resourceId = null,
        object? oldValue = null,
        object? newValue = null,
        string? fieldPath = null)
    {
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        var changeRepo = uow.GetEntityRepository<ProfileChangeRequest>();
        var auditRepo = uow.GetEntityRepository<AuditTrailEntry>();
        var loggerRepo = uow.GetEntityRepository<UserProfileLogger>();
        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();

        var hadPendingChanges = await changeRepo.DbSet
            .AsNoTracking()
            .AnyAsync(c => c.UserProfileId == userProfileId
                           && (c.Status == ProfileChangeRequestStatus.Pending
                               || c.Status == ProfileChangeRequestStatus.UnderReview), ct);

        var targetKey = ProfileChangeRequest.BuildTargetKey(section, targetType, fieldPath, entityName, entityId, resourceId);
        var change = await changeRepo.DbSet
            .Where(c => c.UserProfileId == userProfileId
                        && c.TargetKey == targetKey
                        && (c.Status == ProfileChangeRequestStatus.Pending || c.Status == ProfileChangeRequestStatus.UnderReview))
            .OrderByDescending(c => c.CreatedDate)
            .FirstOrDefaultAsync(ct);

        var createdNewChange = false;

        if (change is null)
        {
            change = ProfileChangeRequest.Create(userProfileId, section, targetType, requestedByUserId, targetKey, fieldPath,
                entityName, entityId, resourceId, attachmentTitle, oldValue, newValue);
            await changeRepo.AddAsync(change);
            createdNewChange = true;
        }
        else
        {
            change.UpdateValues(oldValue, newValue);
            change.AttachmentTitle = attachmentTitle ?? change.AttachmentTitle;
        }

        if (!hadPendingChanges)
        {
            await DeactivateAssignmentsAsync(assignmentRepo, loggerRepo, userProfileId, requestedByUserId, ct,
                UserProfileLogConstants.Notes.ReturnedToDistribution);
        }

        var pending = await reviewRepo.DbSet
            .Where(r => r.UserProfileId == userProfileId
                        && r.Section == section
                        && r.TargetType == targetType
                        && r.ProfileChangeId == change.Id
                        && r.FieldPath == fieldPath
                        && (entityId == null || r.EntityId == entityId)
                        && (resourceId == null || r.ResourceId == resourceId)
                        && r.Status == ReviewStatus.Pending)
            .FirstOrDefaultAsync(ct);

        if (pending is not null)
        {
            await AddAuditEntryAsync(
                auditRepo,
                loggerRepo,
                userProfileId,
                requestedByUserId,
                targetType,
                section,
                fieldPath,
                entityName,
                attachmentTitle,
                entityId,
                resourceId,
                createdNewChange);
            return pending;
        }


        var item = ReviewItem.Create(userProfileId, section, targetType, entityName: entityName, entityId: entityId, resourceId: resourceId);
        item.AttachmentTitle = attachmentTitle;
        item.FieldPath = fieldPath;
        item.ProfileChangeId = change.Id;
        item.IsOutdated = true;

        await reviewRepo.AddAsync(item);
        await AddAuditEntryAsync(
            auditRepo,
            loggerRepo,
            userProfileId,
            requestedByUserId,
            targetType,
            section,
            fieldPath,
            entityName,
            attachmentTitle,
            entityId,
            resourceId,
            createdNewChange);
        return item;
    }

    private static async Task AddAuditEntryAsync(
        IGenericRepository<AuditTrailEntry> auditRepo,
        IGenericRepository<UserProfileLogger> loggerRepo,
        Guid userProfileId,
        Guid requestedByUserId,
        ReviewTargetType targetType,
        ProfileSection section,
        string? fieldPath,
        string? entityName,
        string? attachmentTitle,
        Guid? entityId,
        Guid? resourceId,
        bool createdNewChange)
    {
        var actionLabel = createdNewChange
            ? UserProfileLogConstants.ActionTypes.ProfileChangeRequested
            : UserProfileLogConstants.ActionTypes.ProfileChangeUpdated;
        var note = JsonSerializer.Serialize(new
        {
            eventType = actionLabel,
            targetType = targetType.ToString(),
            section = section.ToString(),
            fieldPath,
            entityName,
            attachmentTitle,
            message = createdNewChange
                ? "A profile change was submitted for review"
                : "A submitted profile change was updated"
        });

        await auditRepo.AddAsync(new AuditTrailEntry
        {
            UserProfileId = userProfileId,
            UserId = requestedByUserId,
            ActionType = actionLabel,
            Notes = note,
            Section = section.ToString(),
            EntityId = entityId,
            AttachmentId = resourceId
        });

        await loggerRepo.AddAsync(new UserProfileLogger
        {
            UserProfileId = userProfileId,
            PerformedById = requestedByUserId,
            ActionType = actionLabel,
            Notes = note,
            Section = section.ToString(),
            EntityId = entityId,
            AttachmentId = resourceId,
            ReviewStatus = ReviewStatus.Pending
        });
    }

    private static async Task DeactivateAssignmentsAsync(
        IGenericRepository<ProfileAssignment> assignmentRepo,
        IGenericRepository<UserProfileLogger> loggerRepo,
        Guid userProfileId,
        Guid performedById,
        CancellationToken ct,
        string note)
    {
        var activeAssignments = await assignmentRepo.DbSet
            .Where(a => a.UserProfileId == userProfileId && a.IsActive)
            .ToListAsync(ct);

        foreach (var assignment in activeAssignments)
        {
            assignment.Deactivate();
            var unassignNote = JsonSerializer.Serialize(new
            {
                eventType = "AssignmentReassigned",
                newAssignedUserId = (Guid?)null,
                newAssignedUserName = (string?)null,
                message = note
            });

            await loggerRepo.AddAsync(new UserProfileLogger
            {
                UserProfileId = assignment.UserProfileId,
                PerformedById = performedById,
                ActionType = UserProfileLogConstants.ActionTypes.ProfileUnassigned,
                Notes = unassignNote,
                Section = "Assignment",
                EntityId = assignment.Id
            });
        }
    }
}
