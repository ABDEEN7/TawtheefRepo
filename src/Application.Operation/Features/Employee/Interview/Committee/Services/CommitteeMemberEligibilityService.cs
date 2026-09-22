using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Interview.Committee.Services;

// Single definition of "who may sit on an interview committee": an active employee user who holds
// InterviewCommittee.View or InterviewCommittee.Manage through one of their roles.
public sealed class CommitteeMemberEligibilityService(UserManager<User> userManager, IUserRepository userRepository)
{
    public async Task<IQueryable<User>> GetEligibleUsersAsync(CancellationToken cancellationToken)
    {
        var viewerIds = await userRepository.GetUserIdsByPermissionAsync(PermissionKeys.InterviewCommittee.View, cancellationToken);
        var managerIds = await userRepository.GetUserIdsByPermissionAsync(PermissionKeys.InterviewCommittee.Manage, cancellationToken);
        var permittedIds = viewerIds.Concat(managerIds).Distinct().ToList();

        return userManager.Users
            .AsNoTracking()
            .OfType<EmployeeUser>()
            .Where(u => !u.IsBlocked && !u.IsDeleted && permittedIds.Contains(u.Id))
            .Cast<User>();
    }

    public async Task<bool> AreAllEligibleAsync(IReadOnlyCollection<Guid> userIds, CancellationToken cancellationToken)
    {
        var distinctIds = userIds.Distinct().ToList();
        if (distinctIds.Count == 0)
            return true;

        var eligibleUsers = await GetEligibleUsersAsync(cancellationToken);
        var eligibleCount = await eligibleUsers.CountAsync(u => distinctIds.Contains(u.Id), cancellationToken);
        return eligibleCount == distinctIds.Count;
    }
}
