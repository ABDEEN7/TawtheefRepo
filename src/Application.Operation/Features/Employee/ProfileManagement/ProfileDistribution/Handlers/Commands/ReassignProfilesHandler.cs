using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Commands;
using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Configurations.Rules;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Handlers.Commands;

public sealed class ReassignProfilesHandler(
    IUnitOfWork uow,
    UserManager<User> userManager,
    IUserRepository userRepository,
    ILocalizationService localizationService,
    IMapper mapper)
    : IRequestHandler<ReassignProfilesCommand, Result<DistributionResultDto>>
{
    public async Task<Result<DistributionResultDto>> Handle(ReassignProfilesCommand request, CancellationToken ct)
    {
        var projection = new ProfileDistributionProjection(uow, userManager, userRepository, localizationService, mapper);
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();
        var changeRepo = uow.GetEntityRepository<ProfileChangeRequest>();
        var auditRepo = uow.GetEntityRepository<AuditTrailEntry>();
        var loggerRepo = uow.GetEntityRepository<UserProfileLogger>();

        var profiles = await profileRepo.DbSet
            .Where(p => request.ProfileIds.Contains(p.Id))
            .ToListAsync(ct);

        if (profiles.Count == 0)
            return Result.Fail<DistributionResultDto>(ErrorsCodes.DistributionProfilesNotFound);

        var profileIds = profiles.Select(p => p.Id).ToList();
        var approvedWithPendingChanges = await changeRepo.DbSet
            .AsNoTracking()
            .Where(c =>
                profileIds.Contains(c.UserProfileId) &&
                (c.Status == ProfileChangeRequestStatus.Pending || c.Status == ProfileChangeRequestStatus.UnderReview))
            .Select(c => c.UserProfileId)
            .Distinct()
            .ToListAsync(ct);
        var approvedWithPendingChangesSet = approvedWithPendingChanges.ToHashSet();

        if (profiles.Any(p =>
                ProfileDistributionRules.FinalStatuses.Contains(p.Status) &&
                !approvedWithPendingChangesSet.Contains(p.Id)))
            return Result.Fail<DistributionResultDto>(ErrorsCodes.DistributionFinalStatusNotAllowed);

        if (profiles.Any(p =>
                !ProfileDistributionRules.AssignableStatuses.Contains(p.Status) &&
                !(p.Status == UserProfileStatus.Approved && approvedWithPendingChangesSet.Contains(p.Id))))
            return Result.Fail<DistributionResultDto>(ErrorsCodes.DistributionStatusNotAssignable);

        var activeAssignments = await assignmentRepo.DbSet
            .Where(a => a.IsActive && request.ProfileIds.Contains(a.UserProfileId))
            .ToListAsync(ct);

        foreach (var assignment in activeAssignments)
        {
            assignment.Deactivate();

            await loggerRepo.AddAsync(new UserProfileLogger
            {
                UserProfileId = assignment.UserProfileId,
                PerformedById = null,
                ActionType = UserProfileLogConstants.ActionTypes.ProfileUnassigned,
                Notes = UserProfileLogConstants.Notes.AssignmentDeactivatedBeforeReassignment,
                Section = UserProfileLogConstants.Sections.Assignment,
                EntityId = assignment.Id
            }, ct);
        }

        foreach (var profile in profiles)
        {
            if (profile.Status != UserProfileStatus.Approved)
                profile.Status = UserProfileStatus.Submitted;
        }

        await uow.SaveChangesAsync(ct);

        var mode = ProfileDistributionModes.Normalize(request.Mode);
        if (mode == ProfileDistributionModes.ManualNormalized)
        {
            var callerUser = await userManager.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == request.UserId, ct);

            User? employee = null;
            if (callerUser is OfficeUser callerOfficeUser && callerOfficeUser.OfficeId is not null)
            {
                employee = await userManager.Users.OfType<OfficeUser>()
                    .FirstOrDefaultAsync(e => e.Id == request.EmployeeId && e.OfficeId == callerOfficeUser.OfficeId && !e.IsDeleted && !e.IsBlocked, ct);
            }
            else
            {
                employee = await userManager.Users.OfType<EmployeeUser>()
                    .FirstOrDefaultAsync(e => e.Id == request.EmployeeId && !e.IsDeleted && !e.IsBlocked, ct);
            }

            if (employee is null)
                return Result.Fail<DistributionResultDto>(ErrorsCodes.DistributionEmployeeNotActive);

            foreach (var profile in profiles)
            {
                if (profile.Status != UserProfileStatus.Approved)
                    profile.Status = UserProfileStatus.UnderReview;

                assignmentRepo.DbSet.Add(ProfileAssignment.Assign(profile.Id, employee.Id));

                await auditRepo.AddAsync(new AuditTrailEntry
                {
                    UserProfileId = profile.Id,
                    UserId = employee.Id,
                    ActionType = UserProfileLogConstants.ActionTypes.ProfileAssigned,
                    Notes = UserProfileLogConstants.Notes.ProfileReassignedManually,
                    Section = UserProfileLogConstants.Sections.Assignment
                }, ct);
                await loggerRepo.AddAsync(new UserProfileLogger
                {
                    UserProfileId = profile.Id,
                    PerformedById = employee.Id,
                    ActionType = UserProfileLogConstants.ActionTypes.ProfileAssigned,
                    Notes = UserProfileLogConstants.Notes.ProfileReassignedManually,
                    Section = UserProfileLogConstants.Sections.Assignment
                }, ct);
            }

            await uow.SaveChangesAsync(ct);
            var result = await projection.BuildResultAsync(request.UserId, profiles.Count, ct);
            return Result.Ok(result);
        }

        if (mode == ProfileDistributionModes.AutoNormalized)
        {
            var autoRequest = new AutoAssignProfilesCommand(
                request.UserId,
                request.EmployeeIds,
                request.ProfileIds,
                request.PerEmployeeCount);

            var handler = new AutoAssignProfilesHandler(uow, userManager, userRepository, localizationService, mapper);
            return await handler.Handle(autoRequest, ct);
        }

        return Result.Fail<DistributionResultDto>(ErrorsCodes.DistributionModeRequired);
    }
}

