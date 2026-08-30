using System.Text.Json;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Commands;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.ProfileApproval;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Handlers.Commands;

public sealed class SaveProfileSectionInternalNoteHandler(IUnitOfWork uow)
    : IRequestHandler<SaveProfileSectionInternalNoteCommand, IResult<Unit>>
{
    private const int MaxNoteLength = 500;

    public async Task<IResult<Unit>> Handle(SaveProfileSectionInternalNoteCommand cmd, CancellationToken ct)
    {
        if (!ProfileApprovalFlow.Sections.Contains(cmd.Section))
            return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

        var note = string.IsNullOrWhiteSpace(cmd.Note) ? null : cmd.Note.Trim();
        if (note?.Length > MaxNoteLength)
            return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

        var profile = await uow.GetEntityRepository<UserProfile>().DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.Id == cmd.UserProfileId, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);
        if (profile.Status != UserProfileStatus.UnderReview)
            return Result.Fail<Unit>(ErrorsCodes.ProfileNotReadyForReview);

        var isAssigned = await uow.GetEntityRepository<ProfileAssignment>().DbSet
            .AsNoTracking()
            .AnyAsync(assignment =>
                assignment.UserProfileId == profile.Id &&
                assignment.EmployeeId == cmd.OfficerId &&
                assignment.IsActive,
                ct);
        if (!isAssigned)
            return Result.Fail<Unit>(ErrorsCodes.UnauthorizedAction);

        var payload = JsonSerializer.Serialize(new
        {
            eventType = UserProfileLogConstants.ActionTypes.ProfileSectionInternalNote,
            section = cmd.Section.ToString(),
            note
        });

        await uow.GetEntityRepository<AuditTrailEntry>().AddAsync(new AuditTrailEntry
        {
            UserProfileId = profile.Id,
            UserId = cmd.OfficerId,
            ActionType = UserProfileLogConstants.ActionTypes.ProfileSectionInternalNote,
            Section = cmd.Section.ToString(),
            Notes = payload
        }, ct);
        await uow.GetEntityRepository<UserProfileLogger>().AddAsync(new UserProfileLogger
        {
            UserProfileId = profile.Id,
            PerformedById = cmd.OfficerId,
            ActionType = UserProfileLogConstants.ActionTypes.ProfileSectionInternalNote,
            Section = cmd.Section.ToString(),
            Notes = payload
        }, ct);

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
