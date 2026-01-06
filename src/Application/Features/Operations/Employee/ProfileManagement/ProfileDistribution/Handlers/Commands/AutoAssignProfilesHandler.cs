using Cortex.Mediator.Commands;
using FluentResults;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.Commands;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.DTOs;
using Tawtheef.Domain.Configurations.Rules;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.Handlers.Commands;

public sealed class AutoAssignProfilesHandler(IUnitOfWork uow, UserManager<User> userManager)
    : ICommandHandler<AutoAssignProfilesCommand, Result<DistributionResultDto>>
{
    public async Task<Result<DistributionResultDto>> Handle(AutoAssignProfilesCommand request, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();
        var changeRepo = uow.GetEntityRepository<ProfileChangeRequest>();
        var auditRepo = uow.GetEntityRepository<AuditTrailEntry>();
        var loggerRepo = uow.GetEntityRepository<UserProfileLogger>();

        var targetEmployeeIds = request.EmployeeIds.ToList();
        var employees = await userManager.Users.OfType<EmployeeUser>()
            .Where(e => targetEmployeeIds.Contains(e.Id) && !e.IsDeleted && !e.IsBlocked)
            .ToListAsync(ct);

        if (employees.Count == 0)
            return Result.Fail<DistributionResultDto>(ErrorsCodes.DistributionNoEligibleEmployees);

        var perEmployeeLimit = request.PerEmployeeCount > 0 ? request.PerEmployeeCount : null;

        var profileQuery = profileRepo.DbSet
            .Where(p =>
                ProfileDistributionRules.AssignableStatuses.Contains(p.Status) ||
                (p.Status == UserProfileStatus.Approved &&
                 changeRepo.DbSet.Any(c =>
                     c.UserProfileId == p.Id &&
                     (c.Status == ProfileChangeRequestStatus.Pending ||
                      c.Status == ProfileChangeRequestStatus.UnderReview))));

        if (request.ProfileIds?.Any() == true)
            profileQuery = profileQuery.Where(p => request.ProfileIds.Contains(p.Id));

        var activeAssignments = await assignmentRepo.DbSet
            .Where(a => a.IsActive)
            .ToListAsync(ct);
        var assignedProfileIds = activeAssignments.Select(a => a.UserProfileId).ToHashSet();

        var assignableProfiles = await profileQuery
            .Where(p => !assignedProfileIds.Contains(p.Id))
            .OrderBy(p => p.CreatedDate)
            .ToListAsync(ct);

        if (assignableProfiles.Count == 0)
            return Result.Fail<DistributionResultDto>(ErrorsCodes.DistributionNoAssignableProfiles);

        var loadLookup = activeAssignments
            .GroupBy(a => a.EmployeeId)
            .ToDictionary(g => g.Key, g => g.Count());

        var newlyAssigned = employees.ToDictionary(e => e.Id, _ => 0);

        foreach (var profile in assignableProfiles)
        {
            var orderedEmployees = employees
                .Select(e => new
                {
                    Employee = e,
                    Load = loadLookup.GetValueOrDefault(e.Id) + newlyAssigned[e.Id],
                    AssignedThisRound = newlyAssigned[e.Id]
                })
                .OrderBy(e => e.Load)
                .ThenBy(e => e.Employee.CreatedDate)
                .ToList();

            var chosen = orderedEmployees.FirstOrDefault(e => perEmployeeLimit is null || e.AssignedThisRound < perEmployeeLimit);
            if (chosen is null)
                break;

            var assignmentResult = chosen.Employee.CreateProfileAssignmentIfAllowed(profile, chosen.Load, chosen.AssignedThisRound, perEmployeeLimit);
            if (assignmentResult.IsFailed)
                continue;

            assignmentRepo.DbSet.Add(assignmentResult.Value);
            await auditRepo.AddAsync(new AuditTrailEntry
            {
                UserProfileId = profile.Id,
                UserId = assignmentResult.Value.EmployeeId,
                ActionType = "ProfileAssigned",
                Notes = "Profile automatically assigned to reviewer",
                Section = "Assignment"
            });
            await loggerRepo.AddAsync(new UserProfileLogger
            {
                UserProfileId = profile.Id,
                PerformedById = assignmentResult.Value.EmployeeId,
                ActionType = "ProfileAssigned",
                Notes = "Profile automatically assigned to reviewer",
                Section = "Assignment",
                EntityId = assignmentResult.Value.Id
            });
            newlyAssigned[chosen.Employee.Id]++;
        }

        var assignedCount = newlyAssigned.Values.Sum();
        await uow.SaveChangesAsync(ct);

        var projection = new ProfileDistributionProjection(uow,userManager);
        var result = await projection.BuildResultAsync(assignedCount, ct);

        return Result.Ok(result);
    }
}
