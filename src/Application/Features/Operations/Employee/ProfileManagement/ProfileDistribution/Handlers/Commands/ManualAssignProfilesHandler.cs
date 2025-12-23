using FluentResults;
using MediatR;
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

public sealed class ManualAssignProfilesHandler(IUnitOfWork uow, UserManager<User> userManager)
    : IRequestHandler<ManualAssignProfilesCommand, Result<DistributionResultDto>>
{
    public async Task<Result<DistributionResultDto>> Handle(ManualAssignProfilesCommand request, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();
        var changeRepo = uow.GetEntityRepository<ProfileChangeRequest>();

        var employee = await userManager.Users.OfType<EmployeeUser>()
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId && !e.IsDeleted && !e.IsBlocked, ct);

        if (employee is null)
            return Result.Fail<DistributionResultDto>(ErrorsCodes.DistributionEmployeeNotActive);

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

        var assignments = await assignmentRepo.DbSet
            .Where(a => a.IsActive && request.ProfileIds.Contains(a.UserProfileId))
            .ToListAsync(ct);

        foreach (var assignment in assignments)
            assignment.Deactivate();

        foreach (var profile in profiles)
        {
            if (profile.Status != UserProfileStatus.Approved)
                profile.Status = UserProfileStatus.UnderReview;

            assignmentRepo.DbSet.Add(ProfileAssignment.Assign(profile.Id, employee.Id));
        }

        await uow.SaveChangesAsync(ct);

        var projection = new ProfileDistributionProjection(uow,userManager);
        var result = await projection.BuildResultAsync(profiles.Count, ct);

        return Result.Ok(result);
    }
}
