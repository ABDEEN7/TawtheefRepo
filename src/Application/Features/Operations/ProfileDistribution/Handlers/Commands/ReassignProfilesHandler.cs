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

public sealed class ReassignProfilesHandler(IUnitOfWork uow, UserManager<User> userManager)
    : IRequestHandler<ReassignProfilesCommand, Result<DistributionResultDto>>
{
    public async Task<Result<DistributionResultDto>> Handle(ReassignProfilesCommand request, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();

        var profiles = await profileRepo.DbSet
            .Where(p => request.ProfileIds.Contains(p.Id))
            .ToListAsync(ct);

        if (profiles.Count == 0)
            return Result.Fail<DistributionResultDto>(ErrorsCodes.DistributionProfilesNotFound);

        if (profiles.Any(p => ProfileDistributionRules.IsFinal(p.Status)))
            return Result.Fail<DistributionResultDto>(ErrorsCodes.DistributionFinalStatusNotAllowed);

        if (profiles.Any(p => !ProfileDistributionRules.IsAssignable(p.Status)))
            return Result.Fail<DistributionResultDto>(ErrorsCodes.DistributionStatusNotAssignable);

        var activeAssignments = await assignmentRepo.DbSet
            .Where(a => a.IsActive && request.ProfileIds.Contains(a.UserProfileId))
            .ToListAsync(ct);

        foreach (var assignment in activeAssignments)
            assignment.Deactivate();

        foreach (var profile in profiles)
            profile.Status = UserProfileStatus.Submitted;

        await uow.SaveChangesAsync(ct);

        var mode = request.Mode?.Trim().ToLowerInvariant();
        if (mode == "manual")
        {
            var employee = await userManager.Users.OfType<EmployeeUser>()
                .FirstOrDefaultAsync(e => e.Id == request.EmployeeId && !e.IsDeleted && !e.IsBlocked, ct);

            if (employee is null)
                return Result.Fail<DistributionResultDto>(ErrorsCodes.DistributionEmployeeNotActive);

            foreach (var profile in profiles)
            {
                profile.Status = UserProfileStatus.UnderReview;
                assignmentRepo.DbSet.Add(ProfileAssignment.Assign(profile.Id, employee.Id));
            }

            await uow.SaveChangesAsync(ct);
            var projection = new ProfileDistributionProjection(uow,userManager);
            var manualResult = await projection.BuildResultAsync(profiles.Count, ct);
            return Result.Ok(manualResult);
        }

        if (mode == "auto")
        {
            var autoRequest = new AutoAssignProfilesCommand(
                request.EmployeeIds,
                request.ProfileIds,
                request.PerEmployeeCount);

            var handler = new AutoAssignProfilesHandler(uow, userManager);
            return await handler.Handle(autoRequest, ct);
        }

        return Result.Fail<DistributionResultDto>(ErrorsCodes.DistributionModeRequired);
    }
}
