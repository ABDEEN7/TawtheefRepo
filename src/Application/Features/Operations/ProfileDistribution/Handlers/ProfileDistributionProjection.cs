using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.ProfileDistribution.DTOs;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.ProfileDistribution.Handlers;

internal sealed class ProfileDistributionProjection(IUnitOfWork uow)
{
    public async Task<IReadOnlyList<DistributionProfileDto>> LoadProfilesAsync(
        UserProfileStatus? status,
        CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();
        var submissionRepo = uow.GetEntityRepository<ProfileSubmission>();

        var profilesQuery = profileRepo.DbSet
            .Include(p => p.User)
            .Include(p => p.CandidateType)
            .Include(p => p.TargetEntity)
            .Where(p => !ProfileDistributionRules.FinalStatuses.Contains(p.Status));

        if (status is not null)
            profilesQuery = profilesQuery.Where(p => p.Status == status);

        var profiles = await profilesQuery.ToListAsync(ct);
        if (profiles.Count == 0) return [];

        var profileIds = profiles.Select(p => p.Id).ToList();

        var submissions = await submissionRepo.DbSet
            .Where(s => profileIds.Contains(s.UserProfileId))
            .GroupBy(s => s.UserProfileId)
            .Select(g => new
            {
                ProfileId = g.Key,
                SubmittedAtUtc = g.OrderByDescending(s => s.Version).Select(s => s.SubmittedAtUtc).FirstOrDefault()
            })
            .ToListAsync(ct);

        var submissionLookup = submissions.ToDictionary(s => s.ProfileId, s => s.SubmittedAtUtc);

        var assignments = await assignmentRepo.DbSet
            .Where(a => a.IsActive && profileIds.Contains(a.UserProfileId))
            .Include(a => a.Employee)
            .ToListAsync(ct);

        var assignmentLookup = assignments.ToDictionary(a => a.UserProfileId, a => a);

        return profiles
            .Select(profile =>
            {
                assignmentLookup.TryGetValue(profile.Id, out var assignment);
                var submittedAt = submissionLookup.TryGetValue(profile.Id, out var submitted)
                    ? submitted
                    : profile.CreatedDate.UtcDateTime;

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
    }

    public async Task<IReadOnlyList<DistributionEmployeeDto>> LoadEmployeesAsync(CancellationToken ct)
    {
        var employeeRepo = uow.GetEntityRepository<EmployeeUser>();
        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();

        var employees = await employeeRepo.DbSet
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
                    Name = emp.FullNameAr ?? emp.FullNameEn,
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
        var profiles = await LoadProfilesAsync(null, ct);

        return new DistributionResultDto
        {
            AssignedCount = assignedCount,
            Employees = employees,
            Profiles = profiles
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
