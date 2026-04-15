using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Configurations.Rules;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Handlers;

internal sealed class ProfileDistributionProjection(
    IUnitOfWork uow,
    UserManager<User> userManager,
    IUserRepository userRepository,
    ILocalizationService localizationService,
    IMapper mapper)
{
    public async Task<PaginatedResult<DistributionProfileDto>> LoadProfilesAsync(
        Guid userId,
        PaginatedRequest paginatedRequest,
        UserProfileStatus? status,
        string? searchTerm,
        Guid? targetEntityId,
        bool? hasOtherSpecialization,
        CancellationToken ct)
    {
        var profileRepo    = uow.GetEntityRepository<UserProfile>();
        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();
        var changeRepo     = uow.GetEntityRepository<ProfileChangeRequest>();

        // Load user + Office navigation safely for OfficeUser
        var user = await userManager.Users
            .Include(u => (u as OfficeUser)!.Office)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null)        
            return new PaginatedResult<DistributionProfileDto>([], 0, paginatedRequest.PageNumber, paginatedRequest.PageSize);


        // 2) Base profiles query with eligibility rules + country filter
        var profilesQuery = profileRepo.DbSet
            .Include(p => p.User)
            .Include(p => p.CandidateType)
            .Include(p => p.TargetEntity)
            .WhereIf(user is EmployeeUser,p=> 
                p.Provider == nameof(ProviderLoginIds.QatarPass) || 
                                              p.Provider == nameof(ProviderLoginIds.QatarResidentOtp))
            .Where(p =>
                (
                    Enumerable.Contains(ProfileDistributionRules.AssignableStatuses, p.Status) ||
                 (p.Status == UserProfileStatus.Approved &&
                  changeRepo.DbSet.Any(c =>
                      c.UserProfileId == p.Id &&
                      (c.Status == ProfileChangeRequestStatus.Pending ||
                       c.Status == ProfileChangeRequestStatus.UnderReview)))
                )
            )
            .Include(p => p.Qualifications!).ThenInclude(q => q.Major)
            .Include(p => p.Qualifications!).ThenInclude(q => q.SubMajor)
            .WhereIf(status is not null, p => p.Status == status);

        if (user is OfficeUser office)
        {
            var allowedCountry = office.Office?.CountryId;
            profilesQuery = profilesQuery
                .Where(p => p.ResidenceCountryId == allowedCountry &&
                            !(p.Provider == nameof(ProviderLoginIds.QatarPass) ||
                              p.Provider == nameof(ProviderLoginIds.QatarResidentOtp)));
        }          

        // 4) Optional search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var trimmed = searchTerm.Trim();
            var term = $"%{trimmed}%";
            var compactTerm = trimmed.Replace(" ", string.Empty);
            var compactLike = $"%{compactTerm}%";
            profilesQuery = profilesQuery.Where(p =>
                (p.User != null &&
                 (EF.Functions.Like(p.User.FullNameAr, term) ||
                  EF.Functions.Like(p.User.FullNameEn, term) ||
                  (compactTerm.Length > 0 &&
                   (EF.Functions.Like(
                        (p.User.FullNameAr).Replace(" ", string.Empty),
                        compactLike) ||
                    EF.Functions.Like(
                        (p.User.FullNameEn).Replace(" ", string.Empty),
                        compactLike))))) ||
                (p.CandidateType != null &&
                 (EF.Functions.Like(p.CandidateType.NameAr, term) ||
                  EF.Functions.Like(p.CandidateType.NameEn, term))) ||
                (p.TargetEntity != null &&
                 (EF.Functions.Like(p.TargetEntity.NameAr, term) ||
                  EF.Functions.Like(p.TargetEntity.NameEn, term))) ||
                EF.Functions.Like(p.NationalNumber ?? string.Empty, term) ||
                assignmentRepo.DbSet.Any(a =>
                    a.IsActive &&
                    a.UserProfileId == p.Id &&
                    a.Employee != null &&
                    (EF.Functions.Like(a.Employee.FullNameAr, term) ||
                     EF.Functions.Like(a.Employee.FullNameEn, term))));
        }

        if (targetEntityId.HasValue)
        {
            profilesQuery = profilesQuery.Where(p => p.TargetEntityId == targetEntityId.Value);
        }

        if (hasOtherSpecialization.HasValue)
        {
            if (hasOtherSpecialization.Value)
            {
                profilesQuery = profilesQuery.Where(p => p.Qualifications!.Any(q => 
                    q.MajorId == MajorIds.Other || q.SubMajorId == MajorIds.SubOther || q.SubMajorId == MajorIds.Other || q.MajorId == MajorIds.SubOther));
            }
            else
            {
                profilesQuery = profilesQuery.Where(p => !p.Qualifications!.Any(q => 
                    q.MajorId == MajorIds.Other || q.SubMajorId == MajorIds.SubOther || q.SubMajorId == MajorIds.Other || q.MajorId == MajorIds.SubOther));
            }
        }

        // 5) Paginate
        var profiles = await profilesQuery.ToPaginatedListAsync(paginatedRequest, ct);
        if (profiles.Metadata.TotalCount == 0)
        {
            return new PaginatedResult<DistributionProfileDto>(
                [],
                profiles.Metadata.TotalCount,
                profiles.Metadata.CurrentPage,
                profiles.Metadata.PageSize);
        }

        // 6) Load active assignments for returned profile IDs (batched)
        var profileIds = profiles.Items.Select(p => p.Id).ToList();

        var assignments = await assignmentRepo.DbSet
            .Where(a => a.IsActive && profileIds.Contains(a.UserProfileId))
            .Include(a => a.Employee)
            .ToListAsync(ct);

        var assignmentLookup = assignments.ToDictionary(a => a.UserProfileId, a => a);

        // 7) Map DTOs
        var items = profiles.Items
            .Select(profile =>
            {
                assignmentLookup.TryGetValue(profile.Id, out var assignment);

                var dto = mapper.Map<DistributionProfileDto>(profile);
                dto.AssignedEmployeeId = assignment?.EmployeeId;
                dto.AssignedEmployeeName = assignment?.Employee == null
                    ? null
                    : localizationService.GetLocalizedFullName(assignment.Employee);
                
                dto.HasOtherSpecialization = profile.Qualifications?.Any(q =>
                    q.MajorId == MajorIds.Other || q.SubMajorId == MajorIds.SubOther || q.SubMajorId == MajorIds.Other || q.SubMajorId == MajorIds.SubOther) ?? false;
                
                return dto;
            })
            .OrderByDescending(p => p.SubmittedAtUtc)
            .ToList();

        return new PaginatedResult<DistributionProfileDto>(
            items,
            profiles.Metadata.TotalCount,
            profiles.Metadata.CurrentPage,
            profiles.Metadata.PageSize);
    }

    public async Task<IReadOnlyList<DistributionEmployeeDto>> LoadEmployeesAsync(Guid userId, CancellationToken ct)
    {
        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();

        var user = await userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null) return [];

        List<User> employees;

        if (user is OfficeUser { OfficeId: not null } officeUser)
        {
            employees = await userManager.Users.OfType<OfficeUser>()
                .Where(u => u.OfficeId == officeUser.OfficeId)
                .AsNoTracking()
                .Cast<User>()
                .ToListAsync(ct);
        }
        else
        {
            employees = await userManager.Users.OfType<EmployeeUser>()
                .AsNoTracking()
                .Cast<User>()
                .ToListAsync(ct);
        }

        if (employees.Count == 0) return [];

        // Filter by permission: "profile.distribution.manage"
        var permEmployees = await userRepository
            .GetUsersByPermissionAsync(PermissionKeys.ProfileDistribution.Manage, ct);
        var permEmployeeIds = permEmployees.Select(u => u.Id).ToHashSet();
        
        employees = employees.Where(e => permEmployeeIds.Contains(e.Id)).ToList();

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
                    Name = localizationService.GetLocalizedFullName(emp),
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
        var employees = await LoadEmployeesAsync(userId, ct);
        var profiles = await LoadProfilesAsync(
            userId: userId,
            paginatedRequest: new PaginatedRequest { PageSize = int.MaxValue },
            status: null,
            searchTerm: null,
            targetEntityId: null,
            hasOtherSpecialization: null,
            ct: ct);

        return new DistributionResultDto
        {
            AssignedCount = assignedCount,
            Employees = employees,
            Profiles = profiles.Items
        };
    }
    
    private static DistributionEmployeeAvailability ResolveAvailability(User employee)
    {
        if (employee.IsBlocked) return DistributionEmployeeAvailability.Suspended;
        if (employee.IsDeleted) return DistributionEmployeeAvailability.Inactive;
        if (employee.LockoutEnd.HasValue && employee.LockoutEnd > DateTimeOffset.UtcNow)
            return DistributionEmployeeAvailability.OnLeave;

        return DistributionEmployeeAvailability.Available;
    }
}
