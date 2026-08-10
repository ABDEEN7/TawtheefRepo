using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Commands;
using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Configurations.Rules;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Profile;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Handlers.Commands;

public sealed class AutoAssignProfilesHandler(
    IUnitOfWork uow,
    UserManager<User> userManager,
    IUserRepository userRepository,
    ILocalizationService localizationService,
    IMapper mapper)
    : IRequestHandler<AutoAssignProfilesCommand, Result<DistributionResultDto>>
{
    public async Task<Result<DistributionResultDto>> Handle(AutoAssignProfilesCommand request, CancellationToken ct)
    {
        var projection = new ProfileDistributionProjection(uow, userManager, userRepository, localizationService, mapper);
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();
        var changeRepo = uow.GetEntityRepository<ProfileChangeRequest>();
        var auditRepo = uow.GetEntityRepository<AuditTrailEntry>();
        var loggerRepo = uow.GetEntityRepository<UserProfileLogger>();

        var targetEmployeeIds = request.EmployeeIds.ToList();

        var callerUser = await userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.UserId, ct);

        List<User> employees;

        if (callerUser is OfficeUser { OfficeId: not null } callerOfficeUser)
        {
            employees = await userManager.Users.OfType<OfficeUser>()
                .Where(e => targetEmployeeIds.Contains(e.Id) && e.OfficeId == callerOfficeUser.OfficeId && !e.IsDeleted && !e.IsBlocked)
                .Cast<User>()
                .ToListAsync(ct);
        }
        else
        {
            employees = await userManager.Users.OfType<EmployeeUser>()
                .Where(e => targetEmployeeIds.Contains(e.Id) && !e.IsDeleted && !e.IsBlocked)
                .Cast<User>()
                .ToListAsync(ct);
        }

        if (employees.Count == 0)
            return Result.Fail<DistributionResultDto>(ErrorsCodes.DistributionNoEligibleEmployees);

        // Filter by permission: "profile.distribution.manage"
        var permEmployees = await userRepository.GetUsersByPermissionAsync(PermissionKeys.ProfileDistribution.Manage, ct);
        var permEmployeeIds = permEmployees.Select(u => u.Id).ToHashSet();
        employees = employees.Where(e => permEmployeeIds.Contains(e.Id)).ToList();

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
        var notificationAssignments = new Dictionary<Guid, ProfileAssignment>();

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

            var assignmentResult = chosen.Employee.CreateProfileAssignmentIfAllowed(
                profile,
                chosen.Load,
                chosen.AssignedThisRound,
                perEmployeeLimit,
                publishNotification: false);
            if (assignmentResult.IsFailed)
                continue;

            var assignmentNote = JsonSerializer.Serialize(new
            {
                eventType = "AssignmentCreated",
                assignmentMode = "Auto",
                newAssignedUserId = assignmentResult.Value.EmployeeId,
                newAssignedUserName = chosen.Employee.FullNameEn,
                message = UserProfileLogConstants.Notes.ProfileAssignedAutomatically
            });

            assignmentRepo.DbSet.Add(assignmentResult.Value);
            notificationAssignments.TryAdd(chosen.Employee.Id, assignmentResult.Value);
            await auditRepo.AddAsync(new AuditTrailEntry
            {
                UserProfileId = profile.Id,
                UserId = assignmentResult.Value.EmployeeId,
                ActionType = UserProfileLogConstants.ActionTypes.ProfileAssigned,
                Notes = assignmentNote,
                Section = UserProfileLogConstants.Sections.Assignment
            }, ct);
            await loggerRepo.AddAsync(new UserProfileLogger
            {
                UserProfileId = profile.Id,
                PerformedById = assignmentResult.Value.EmployeeId,
                ActionType = UserProfileLogConstants.ActionTypes.ProfileAssigned,
                Notes = assignmentNote,
                Section = UserProfileLogConstants.Sections.Assignment,
                EntityId = assignmentResult.Value.Id
            }, ct);

            newlyAssigned[chosen.Employee.Id]++;
        }

        foreach (var (employeeId, assignment) in notificationAssignments)
        {
            assignment.AddDomainEvent(new ProfileAssignedEvent(
                assignment.UserProfileId,
                employeeId,
                DateTimeOffset.UtcNow,
                newlyAssigned[employeeId]));
        }

        var assignedCount = newlyAssigned.Values.Sum();
        await uow.SaveChangesAsync(ct);

        var result = await projection.BuildResultAsync(request.UserId, assignedCount, ct);

        return Result.Ok(result);
    }
}

