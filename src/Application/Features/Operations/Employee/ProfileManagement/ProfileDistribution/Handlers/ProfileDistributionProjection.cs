using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.DTOs;
using Tawtheef.Domain.Configurations.Rules;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.Handlers;

internal sealed class ProfileDistributionProjection(IUnitOfWork uow, UserManager<User> userManager)
{
    public async Task<PaginatedResult<DistributionProfileDto>> LoadProfilesAsync(
        PaginatedRequest paginatedRequest,
        UserProfileStatus? status,
        CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();
        var changeRepo = uow.GetEntityRepository<ProfileChangeRequest>();

        var profilesQuery = profileRepo.DbSet
            .Include(p => p.User)
            .Include(p => p.CandidateType)
            .Include(p => p.TargetEntity)
            .Where(p =>
                (
                    !Enumerable.Contains(ProfileDistributionRules.StartStatuses, p.Status) &&
                    !Enumerable.Contains(ProfileDistributionRules.FinalStatuses, p.Status)
                )
                || (p.Status == UserProfileStatus.Approved &&
                    changeRepo.DbSet.Any(c =>
                        c.UserProfileId == p.Id &&
                        (c.Status == ProfileChangeRequestStatus.Pending ||
                         c.Status == ProfileChangeRequestStatus.UnderReview)
                    )
                )
            );

        if (status is not null)
            profilesQuery = profilesQuery.Where(p => p.Status == status);

        var profiles = await profilesQuery
            .ToPaginatedListAsync(paginatedRequest, ct);
        if (profiles.Metadata.TotalCount == 0)
            return new PaginatedResult<DistributionProfileDto>(
                [],
                profiles.Metadata.TotalCount,
                profiles.Metadata.CurrentPage,
                profiles.Metadata.PageSize);

        var profileIds = profiles.Items.Select(p => p.Id).ToList();
        var assignments = await assignmentRepo.DbSet
            .Where(a => a.IsActive && profileIds.Contains(a.UserProfileId))
            .Include(a => a.Employee)
            .ToListAsync(ct);

        var assignmentLookup = assignments.ToDictionary(a => a.UserProfileId, a => a);

        var items = profiles.Items
            .Select(profile =>
            {
                assignmentLookup.TryGetValue(profile.Id, out var assignment);
                var submittedAt = profile.CreatedDate;
                var candidateName = profile.User?.FullNameAr ?? profile.User?.FullNameEn ?? string.Empty;
                var specialization = profile.CandidateType?.NameAr ?? profile.CandidateType?.NameEn ?? string.Empty;
                var target = profile.TargetEntity?.NameAr ?? profile.TargetEntity?.NameEn ?? string.Empty;

                return new DistributionProfileDto
                {
                    ProfileId = profile.Id,
                    CandidateName = candidateName,
                    Specialization = specialization,
                    TargetEntity = target,
                    Status = profile.Status,
                    AssignedEmployeeId = assignment?.EmployeeId,
                    AssignedEmployeeName = assignment?.Employee?.FullNameAr ?? assignment?.Employee?.FullNameEn,
                    SubmittedAtUtc = submittedAt
                };
            })
            .OrderByDescending(p => p.SubmittedAtUtc)
            .ToList();

        return new PaginatedResult<DistributionProfileDto>(
            items,
            profiles.Metadata.TotalCount,
            profiles.Metadata.CurrentPage,
            profiles.Metadata.PageSize);
    }

    public async Task<IReadOnlyList<DistributionEmployeeDto>> LoadEmployeesAsync(CancellationToken ct)
    {
        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();

        var employees = await userManager.Users.OfType<EmployeeUser>()
            .Where(e => !e.IsDeleted)
            .ToListAsync(ct);

        if (employees.Count == 0) return [];

        var employeeIds = employees.Select(e => e.Id).ToList();

        var assignments = await assignmentRepo.DbSet
            .Where(a => a.IsActive && employeeIds.Contains(a.EmployeeId))
            .Include(a => a.UserProfile)
            .ToListAsync(ct);

        var loadLookup = assignments
            .GroupBy(a => a.EmployeeId)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    Total = g.Count(),
                    Completed = g.Count(a => a.UserProfile?.Status == UserProfileStatus.Approved),
                    InReview = g.Count(a => a.UserProfile?.Status == UserProfileStatus.UnderReview)
                });

        return employees
            .Select(emp =>
            {
                var availability = ResolveAvailability(emp);
                loadLookup.TryGetValue(emp.Id, out var load);

                return new DistributionEmployeeDto
                {
                    EmployeeId = emp.Id,
                    Name = emp.FullNameAr,
                    TotalAssigned = load?.Total ?? 0,
                    Completed = load?.Completed ?? 0,
                    InReview = load?.InReview ?? 0,
                    IsActive = !emp.IsBlocked && !emp.IsDeleted,
                    Availability = availability
                };
            })
            .OrderBy(e => e.Name)
            .ToList();
    }

    public async Task<DistributionResultDto> BuildResultAsync(int assignedCount, CancellationToken ct)
    {
        var employees = await LoadEmployeesAsync(ct);
        var profiles = await LoadProfilesAsync(new PaginatedRequest {PageSize = int.MaxValue}, null, ct);

        return new DistributionResultDto
        {
            AssignedCount = assignedCount,
            Employees = employees,
            Profiles = profiles.Items
        };
    }

    private static DistributionEmployeeAvailability ResolveAvailability(EmployeeUser employee)
    {
        if (employee.IsBlocked) return DistributionEmployeeAvailability.Suspended;
        if (employee.IsDeleted) return DistributionEmployeeAvailability.Inactive;
        if (employee.LockoutEnd.HasValue && employee.LockoutEnd > DateTimeOffset.UtcNow)
            return DistributionEmployeeAvailability.OnLeave;

        return DistributionEmployeeAvailability.Available;
    }
}
