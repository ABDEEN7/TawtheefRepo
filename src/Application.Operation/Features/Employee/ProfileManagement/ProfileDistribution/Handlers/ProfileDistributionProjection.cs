using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;
using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Queries;
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
using Tawtheef.Domain.Entities.MinisterOffice;
using Tawtheef.Domain.Constants;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Handlers;

public sealed class ProfileDistributionProjection(
    IUnitOfWork uow,
    UserManager<User> userManager,
    IUserRepository userRepository,
    ILocalizationService localizationService,
    IMapper mapper)
{
    public async Task<PaginatedResult<DistributionProfileDto>> LoadProfilesAsync(
        Guid userId,
        GetDistributionProfilesQuery request,
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
            return new PaginatedResult<DistributionProfileDto>([], 0, request.PageNumber, request.PageSize);


        // 2) Base profiles query with eligibility rules + country filter
        // Keep the paging query free of Includes. Joining collection navigations before
        // Skip/Take multiplies profile rows and becomes very expensive for large pages.
        var profilesQuery = profileRepo.DbSet
            .AsNoTracking()
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
            );

        if (user is OfficeUser office)
        {
            var allowedCountry = office.Office?.CountryId;
            profilesQuery = profilesQuery
                .Where(p => p.ResidenceCountryId == allowedCountry && p.Provider == nameof(ProviderLoginIds.Google));
        }          

        // 4) Optional search filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var trimmed = request.SearchTerm.Trim();
            var term = $"%{trimmed}%";
            var compactTerm = trimmed.Replace(" ", string.Empty);
            var compactLike = $"%{compactTerm}%";
            profilesQuery = profilesQuery.Where(p =>
                p.User != null &&
                 (EF.Functions.Like(p.User.FullNameAr, term) ||
                  EF.Functions.Like(p.User.FullNameEn, term) ||
                  (compactTerm.Length > 0 &&
                   (EF.Functions.Like(
                        (p.User.FullNameAr).Replace(" ", string.Empty),
                        compactLike) ||
                    EF.Functions.Like(
                        (p.User.FullNameEn).Replace(" ", string.Empty),
                        compactLike)))) ||
                EF.Functions.Like(p.NationalNumber ?? string.Empty, term));
        }

        profilesQuery = ApplyFilters(profilesQuery, request, assignmentRepo.DbSet);

        profilesQuery = ApplySorting(profilesQuery, request);

        // 5) Paginate after scope, eligibility, filters, and deterministic sorting.
        var profiles = await PaginateAsync(profilesQuery, request, ct);
        if (profiles.Metadata.TotalCount == 0)
        {
            return new PaginatedResult<DistributionProfileDto>(
                [],
                profiles.Metadata.TotalCount,
                profiles.Metadata.CurrentPage,
                profiles.Metadata.PageSize);
        }

        // 6) Load the navigation data only for the profiles in the requested page.
        // Split queries avoid a cartesian result when collection navigations are included.
        var profileIds = profiles.Items.Select(p => p.Id).ToList();

        var profileDetails = await profileRepo.DbSet
            .Where(p => profileIds.Contains(p.Id))
            .Include(p => p.User)
            .Include(p => p.CandidateType)
            .Include(p => p.TargetEntity)
            .Include(p => p.Qualifications)
            .AsSplitQuery()
            .AsNoTracking()
            .ToListAsync(ct);

        var profileDetailsById = profileDetails.ToDictionary(p => p.Id);
        var orderedProfiles = profiles.Items
            .Select(p => profileDetailsById[p.Id])
            .ToList();

        var assignments = await assignmentRepo.DbSet
            .Where(a => a.IsActive && profileIds.Contains(a.UserProfileId))
            .Include(a => a.Employee)
            .ToListAsync(ct);

        var assignmentLookup = assignments.ToDictionary(a => a.UserProfileId, a => a);

        // 7.1) Load Minister Office Candidates for identifying them (batched)
        var qids = orderedProfiles.Select(p => p.NationalNumber).Where(q => q != null).ToList();
        var ministerOfficeQids = await uow.GetEntityRepository<MinisterOfficeCandidate>().DbSet
            .Where(c => qids.Contains(c.Qid))
            .Select(c => c.Qid)
            .ToListAsync(ct);
        var ministerOfficeLookup = ministerOfficeQids.ToHashSet();

        // 8) Map DTOs
        var items = orderedProfiles
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
                dto.HasOtherUniversity = profile.Qualifications?.Any(q =>
                    !q.IsDeleted && UniversityIds.IsOther(q.UniversityId)) ?? false;

                dto.IsMinisterOfficeCandidate = profile.NationalNumber != null && ministerOfficeLookup.Contains(profile.NationalNumber);
                
                return dto;
            })
            .ToList();

        return new PaginatedResult<DistributionProfileDto>(
            items,
            profiles.Metadata.TotalCount,
            profiles.Metadata.CurrentPage,
            profiles.Metadata.PageSize);
    }

    private static IQueryable<UserProfile> ApplyFilters(
        IQueryable<UserProfile> query,
        GetDistributionProfilesQuery request,
        IQueryable<ProfileAssignment> assignments)
    {
        var statuses = request.Statuses?.Distinct().ToArray() ?? [];
        if (statuses.Length > 0)
            query = query.Where(p => statuses.Contains(p.Status));

        var candidateTypeIds = request.CandidateTypeIds?.Distinct().ToArray() ?? [];

        query = query
            .WhereIf(request.AssignedEmployeeId.HasValue, p => assignments.Any(a =>
                a.IsActive && a.UserProfileId == p.Id && a.EmployeeId == request.AssignedEmployeeId))
            .WhereIf(request.TargetEntityId.HasValue, p => p.TargetEntityId == request.TargetEntityId)
            .WhereIf(candidateTypeIds.Length > 0, p =>
                p.CandidateTypeId.HasValue && candidateTypeIds.Contains(p.CandidateTypeId.Value));

        if (request.HasOtherSpecialization.HasValue)
            query = request.HasOtherSpecialization.Value
                ? query.Where(p => p.Qualifications!.Any(q =>
                    q.MajorId == MajorIds.Other || q.MajorId == MajorIds.SubOther ||
                    q.SubMajorId == MajorIds.Other || q.SubMajorId == MajorIds.SubOther))
                : query.Where(p => !p.Qualifications!.Any(q =>
                    q.MajorId == MajorIds.Other || q.MajorId == MajorIds.SubOther ||
                    q.SubMajorId == MajorIds.Other || q.SubMajorId == MajorIds.SubOther));

        if (request.HasOtherUniversity.HasValue)
            query = request.HasOtherUniversity.Value
                ? query.Where(p => p.Qualifications!.Any(q => q.UniversityId == UniversityIds.Other))
                : query.Where(p => p.Qualifications!.All(q => q.UniversityId != UniversityIds.Other));

        if (request.IsQatarGraduate)
        {
            query = query.Where(p => p.Qualifications!.Any(q =>
                !q.IsDeleted && q.CountryId == CountryIds.Qatar));

            foreach (var degreeId in request.DegreeIds?.Distinct() ?? [])
            {
                var requiredDegreeId = degreeId;
                query = query.Where(p => p.Qualifications!.Any(q =>
                    !q.IsDeleted &&
                    q.CountryId == CountryIds.Qatar &&
                    q.DegreeId == requiredDegreeId));
            }
        }

        return query;
    }

    private static IQueryable<UserProfile> ApplySorting(IQueryable<UserProfile> query, PaginatedRequest request)
    {
        var descending = !string.Equals(request.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
        return request.SortBy?.Trim().ToLowerInvariant() switch
        {
            "createddate" => descending
                ? query.OrderByDescending(p => p.CreatedDate).ThenByDescending(p => p.Id)
                : query.OrderBy(p => p.CreatedDate).ThenBy(p => p.Id),
            _ => query.OrderByDescending(p => p.CreatedDate).ThenByDescending(p => p.Id)
        };
    }

    private static async Task<PaginatedResult<UserProfile>> PaginateAsync(
        IQueryable<UserProfile> query, PaginatedRequest request, CancellationToken ct)
    {
        var count = await query.CountAsync(ct);
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);
        return new PaginatedResult<UserProfile>(items, count, request.PageNumber, request.PageSize);
    }

    public async Task<PaginatedResult<DistributionEmployeeDto>> LoadEmployeesAsync(
        GetDistributionEmployeesQuery request,
        CancellationToken ct)
    {
        var employees = await BuildEligibleEmployeeQueryAsync(request.UserId, ct);
        if (employees is null)
            return new PaginatedResult<DistributionEmployeeDto>([], 0, request.PageNumber, request.PageSize);

        employees = ApplyEmployeeSearch(employees, request.SearchTerm);
        var employeeBase = ApplyAvailabilityFilter(
            ProjectEmployeeBase(employees, request.Language), request.Availability);
        var projected = ProjectEmployees(employeeBase);

        var totalCount = await projected.CountAsync(ct);
        var sorted = ApplyEmployeeSorting(projected, request);
        var rows = await sorted
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(ToEmployeeDto())
            .ToListAsync(ct);

        return new PaginatedResult<DistributionEmployeeDto>(
            rows, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<IReadOnlyList<DistributionEmployeeLookupDto>> LoadEmployeeLookupAsync(
        Guid userId,
        CancellationToken ct)
    {
        var employees = await BuildEligibleEmployeeQueryAsync(userId, ct);
        if (employees is null) return [];

        return await ProjectEmployeeBase(employees, localizationService.GetCurrentLanguage())
            .OrderBy(employee => employee.Name)
            .Select(employee => new DistributionEmployeeLookupDto
            {
                EmployeeId = employee.Employee.Id,
                Name = employee.Name,
                Email = employee.Employee.Email ?? string.Empty,
                IsActive = employee.IsActive,
                Availability = employee.Availability
            })
            .ToListAsync(ct);
    }

    private async Task<IQueryable<User>?> BuildEligibleEmployeeQueryAsync(Guid userId, CancellationToken ct)
    {
        var user = await userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user is null) return null;

        IQueryable<User> employees = user is OfficeUser { OfficeId: not null } officeUser
            ? userManager.Users.OfType<OfficeUser>().Where(u => u.OfficeId == officeUser.OfficeId).Cast<User>()
            : userManager.Users.OfType<EmployeeUser>().Cast<User>();

        // Review permission is the eligibility rule for employees who can receive profiles.
        var permittedEmployeeIds = await userRepository
            .GetUserIdsByPermissionAsync(PermissionKeys.ProfileApproval.Review, ct);
        return employees.AsNoTracking().Where(employee => permittedEmployeeIds.Contains(employee.Id));
    }

    private static IQueryable<EmployeeBaseRow> ProjectEmployeeBase(
        IQueryable<User> employees,
        string? language)
    {
        var isArabic = string.Equals(language, "ar", StringComparison.OrdinalIgnoreCase);
        return employees.Select(employee => new EmployeeBaseRow
        {
            Employee = employee,
            Name = isArabic ? employee.FullNameAr : employee.FullNameEn,
            IsActive = !employee.IsBlocked && !employee.IsDeleted,
            Availability = employee.IsBlocked
                ? DistributionEmployeeAvailability.Suspended
                : employee.IsDeleted
                    ? DistributionEmployeeAvailability.Inactive
                    : employee.LockoutEnd.HasValue && employee.LockoutEnd > DateTimeOffset.UtcNow
                        ? DistributionEmployeeAvailability.OnLeave
                        : DistributionEmployeeAvailability.Available
        });
    }

    private IQueryable<EmployeeLoadRow> ProjectEmployees(IQueryable<EmployeeBaseRow> employees)
    {
        var assignments = uow.GetEntityRepository<ProfileAssignment>().DbSet;
        return employees.Select(employee => new EmployeeLoadRow
        {
            Employee = employee.Employee,
            Name = employee.Name,
            IsActive = employee.IsActive,
            Availability = employee.Availability,
            TotalAssigned = assignments.Count(a => a.IsActive && a.EmployeeId == employee.Employee.Id),
            Completed = assignments.Count(a => a.IsActive && a.EmployeeId == employee.Employee.Id &&
                a.UserProfile != null && a.UserProfile.Status == UserProfileStatus.Approved),
            InReview = assignments.Count(a => a.IsActive && a.EmployeeId == employee.Employee.Id &&
                a.UserProfile != null && a.UserProfile.Status == UserProfileStatus.UnderReview)
        });
    }

    private static System.Linq.Expressions.Expression<Func<EmployeeLoadRow, DistributionEmployeeDto>> ToEmployeeDto() =>
        row => new DistributionEmployeeDto
        {
            EmployeeId = row.Employee.Id,
            Name = row.Name,
            Email = row.Employee.Email ?? string.Empty,
            TotalAssigned = row.TotalAssigned,
            Completed = row.Completed,
            InReview = row.InReview,
            IsActive = row.IsActive,
            Availability = row.Availability
        };

    private static IQueryable<User> ApplyEmployeeSearch(
        IQueryable<User> employees,
        string? searchTerm)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = $"%{searchTerm.Trim()}%";
            employees = employees.Where(employee =>
                EF.Functions.Like(employee.FullNameAr, term) ||
                EF.Functions.Like(employee.FullNameEn, term) ||
                EF.Functions.Like(employee.Email ?? string.Empty, term));
        }

        return employees;
    }

    private static IQueryable<EmployeeBaseRow> ApplyAvailabilityFilter(
        IQueryable<EmployeeBaseRow> employees,
        DistributionEmployeeAvailability? availability) =>
        availability.HasValue
            ? employees.Where(employee => employee.Availability == availability.Value)
            : employees;

    private static IOrderedQueryable<EmployeeLoadRow> ApplyEmployeeSorting(
        IQueryable<EmployeeLoadRow> employees,
        GetDistributionEmployeesQuery request)
    {
        var descending = string.Equals(request.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        return string.Equals(request.SortBy, "TotalAssigned", StringComparison.OrdinalIgnoreCase)
            ? descending
                ? employees.OrderByDescending(employee => employee.TotalAssigned).ThenBy(employee => employee.Name)
                : employees.OrderBy(employee => employee.TotalAssigned).ThenBy(employee => employee.Name)
            : descending
                ? employees.OrderByDescending(employee => employee.Name)
                : employees.OrderBy(employee => employee.Name);
    }

    private sealed class EmployeeBaseRow
    {
        public required User Employee { get; init; }
        public required string Name { get; init; }
        public bool IsActive { get; init; }
        public DistributionEmployeeAvailability Availability { get; init; }
    }

    private sealed class EmployeeLoadRow
    {
        public required User Employee { get; init; }
        public required string Name { get; init; }
        public bool IsActive { get; init; }
        public DistributionEmployeeAvailability Availability { get; init; }
        public int TotalAssigned { get; init; }
        public int Completed { get; init; }
        public int InReview { get; init; }
    }

    public async Task<DistributionResultDto> BuildResultAsync(Guid userId, int assignedCount, CancellationToken ct)
    {
        var eligibleEmployees = await BuildEligibleEmployeeQueryAsync(userId, ct);
        IReadOnlyList<DistributionEmployeeDto> employees = eligibleEmployees is null
            ? []
            : await ProjectEmployees(ProjectEmployeeBase(
                    eligibleEmployees, localizationService.GetCurrentLanguage()))
                .OrderBy(employee => employee.Name)
                .Select(ToEmployeeDto())
                .ToListAsync(ct);
        var profiles = await LoadProfilesAsync(
            userId: userId,
            request: new GetDistributionProfilesQuery(userId) { PageSize = int.MaxValue },
            ct: ct);

        return new DistributionResultDto
        {
            AssignedCount = assignedCount,
            Employees = employees,
            Profiles = profiles.Items
        };
    }
    
}
