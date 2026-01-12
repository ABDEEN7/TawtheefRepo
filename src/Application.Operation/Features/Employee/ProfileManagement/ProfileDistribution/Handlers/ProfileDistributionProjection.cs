using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Configurations.Rules;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Handlers;

internal sealed class ProfileDistributionProjection(IUnitOfWork uow, UserManager<User> userManager)
{
    public async Task<PaginatedResult<DistributionProfileDto>> LoadProfilesAsync(
        Guid userId,
        PaginatedRequest paginatedRequest,
        UserProfileStatus? status,
        CancellationToken ct)
    {
        var profileRepo    = uow.GetEntityRepository<UserProfile>();
        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();
        var changeRepo     = uow.GetEntityRepository<ProfileChangeRequest>();

        // 1) Resolve allowed country for current user (EmployeeUser => Qatar, OfficeUser => Office.CountryId)
        var allowedCountryId = await ResolveAllowedCountryIdAsync(userId, ct);
        if (allowedCountryId is null)            
            return new PaginatedResult<DistributionProfileDto>([], 0, paginatedRequest.PageNumber, paginatedRequest.PageSize);


        // 2) Base profiles query with eligibility rules + country filter
        var profilesQuery = profileRepo.DbSet
            .Include(p => p.User)
            .Include(p => p.CandidateType)
            .Include(p => p.TargetEntity)
            #if !DEBUG
            .Where(p=> p.ResidenceCountryId == allowedCountryId.Value)
            #endif
            .Where(p =>
                (
                    Enumerable.Contains(ProfileDistributionRules.AssignableStatuses, p.Status) ||
                 (p.Status == UserProfileStatus.Approved &&
                  changeRepo.DbSet.Any(c =>
                      c.UserProfileId == p.Id &&
                      (c.Status == ProfileChangeRequestStatus.Pending ||
                       c.Status == ProfileChangeRequestStatus.UnderReview)))
                )
            );

        // 3) Optional status filter
        if (status is not null)
            profilesQuery = profilesQuery.Where(p => p.Status == status);

        // 4) Paginate
        var profiles = await profilesQuery.ToPaginatedListAsync(paginatedRequest, ct);
        if (profiles.Metadata.TotalCount == 0)
        {
            return new PaginatedResult<DistributionProfileDto>(
                [],
                profiles.Metadata.TotalCount,
                profiles.Metadata.CurrentPage,
                profiles.Metadata.PageSize);
        }

        // 5) Load active assignments for returned profile IDs (batched)
        var profileIds = profiles.Items.Select(p => p.Id).ToList();

        var assignments = await assignmentRepo.DbSet
            .Where(a => a.IsActive && profileIds.Contains(a.UserProfileId))
            .Include(a => a.Employee)
            .ToListAsync(ct);

        var assignmentLookup = assignments.ToDictionary(a => a.UserProfileId, a => a);

        // 6) Map DTOs
        var items = profiles.Items
            .Select(profile =>
            {
                assignmentLookup.TryGetValue(profile.Id, out var assignment);

                var submittedAt     = profile.CreatedDate;
                var candidateName   = profile.User?.FullNameAr ?? profile.User?.FullNameEn ?? string.Empty;
                var specialization  = profile.CandidateType?.NameAr ?? profile.CandidateType?.NameEn ?? string.Empty;
                var target          = profile.TargetEntity?.NameAr ?? profile.TargetEntity?.NameEn ?? string.Empty;

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
                    IsActive = emp is { IsBlocked: false, IsDeleted: false },
                    Availability = availability
                };
            })
            .OrderBy(e => e.Name)
            .ToList();
    }

    public async Task<DistributionResultDto> BuildResultAsync(Guid userId, int assignedCount, CancellationToken ct)
    {
        var employees = await LoadEmployeesAsync(ct);

        // Note: this overload must exist in your codebase; keeping your original intent.
        var profiles = await LoadProfilesAsync(
            userId: userId, // replace with actual current userId if needed by your workflow
            paginatedRequest: new PaginatedRequest { PageSize = int.MaxValue },
            status: null,
            ct: ct);

        return new DistributionResultDto
        {
            AssignedCount = assignedCount,
            Employees = employees,
            Profiles = profiles.Items
        };
    }

    private async Task<Guid?> ResolveAllowedCountryIdAsync(Guid userId, CancellationToken ct)
    {
        // Load user + Office navigation safely for OfficeUser
        var user = await userManager.Users
            .Include(u => (u as OfficeUser)!.Office)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null)
            return null;

        return user switch
        {
            // EmployeeUser => Qatar only
            EmployeeUser => CountryIds.Qatar,

            // OfficeUser => Office.CountryId
            OfficeUser officeUser when officeUser.Office is not null => officeUser.Office.CountryId,

            _ => null
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
