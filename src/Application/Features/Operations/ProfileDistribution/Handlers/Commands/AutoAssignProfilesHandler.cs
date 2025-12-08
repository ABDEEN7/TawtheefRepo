using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.ProfileDistribution.Commands;
using Tawtheef.Application.Features.Operations.ProfileDistribution.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.ProfileDistribution.Handlers.Commands;

public sealed class AutoAssignProfilesHandler(IUnitOfWork uow, UserManager<User> userManager)
    : IRequestHandler<AutoAssignProfilesCommand, Result<DistributionResultDto>>
{
    public async Task<Result<DistributionResultDto>> Handle(AutoAssignProfilesCommand request, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();

        var targetEmployeeIds = request.EmployeeIds.ToList() ?? [];
        var employees = await userManager.Users.OfType<EmployeeUser>()
            .Where(e => targetEmployeeIds.Contains(e.Id) && !e.IsDeleted && !e.IsBlocked)
            .ToListAsync(ct);

        if (employees.Count == 0)
            return Result.Fail<DistributionResultDto>(ErrorsCodes.DistributionNoEligibleEmployees);

        var perEmployeeLimit = request.PerEmployeeCount > 0 ? request.PerEmployeeCount : null;

        var profileQuery = profileRepo.DbSet
            .Where(p => ProfileDistributionRules.IsAssignable(p.Status));

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

            profile.Status = UserProfileStatus.UnderReview;
            assignmentRepo.DbSet.Add(ProfileAssignment.Assign(profile.Id, chosen.Employee.Id));
            newlyAssigned[chosen.Employee.Id]++;
        }

        var assignedCount = newlyAssigned.Values.Sum();
        await uow.SaveChangesAsync(ct);

        var projection = new ProfileDistributionProjection(uow,userManager);
        var result = await projection.BuildResultAsync(assignedCount, ct);

        return Result.Ok(result);
    }
}
