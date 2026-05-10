using Application.Operation.Features.Employee.Dashboard.DTOs;
using Application.Operation.Features.Employee.Dashboard.Queries;
using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Handlers;
using FluentResults;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Common.Security;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Dashboard.Services;

public interface IDashboardReadService
{
    Task<Result<DashboardOverviewDto>> GetOverviewAsync(DashboardQueryBase request, CancellationToken ct);
    Task<Result<CandidateStatusSummaryDto>> GetCandidateStatusAsync(DashboardQueryBase request, CancellationToken ct);
    Task<Result<CandidateTypeSummaryDto>> GetCandidateTypesAsync(DashboardQueryBase request, CancellationToken ct);
    Task<Result<JobsSummaryDto>> GetJobsSummaryAsync(DashboardQueryBase request, CancellationToken ct);
    Task<Result<IReadOnlyList<LatestJobDto>>> GetLatestJobsAsync(DashboardQueryBase request, CancellationToken ct);
    Task<Result<EmployeeIndicatorsDto>> GetEmployeeIndicatorsAsync(DashboardQueryBase request, CancellationToken ct);
    Task<Result<EmployeeReviewOutcomesDto>> GetEmployeeReviewOutcomesAsync(DashboardQueryBase request, CancellationToken ct);
    Task<Result<PaginatedResult<TeamPerformanceRowDto>>> GetTeamPerformanceAsync(GetTeamPerformanceQuery request, CancellationToken ct);
}

public sealed class DashboardReadService(
    IUnitOfWork uow,
    UserManager<User> userManager,
    ILocalizationService localizationService,
    ICurrentUserService currentUserService,
    IUserRepository userRepository,
    IMapper mapper) : IDashboardReadService
{
    private const int DefaultLookbackDays = 30;
    private const int OverdueAfterDays = 3;
    private const int MaxTopItems = 8;

    public async Task<Result<DashboardOverviewDto>> GetOverviewAsync(DashboardQueryBase request, CancellationToken ct)
    {
        var contextResult = await CreateContextAsync(request, ct);
        if (contextResult.IsFailed) return Result.Fail(contextResult.Errors);

        var context = contextResult.Value;
        var profileQuery = BuildProfilesQuery(request, context);
        var jobsQuery = BuildJobsQuery(request, context);
        var assignmentsQuery = BuildAssignmentsQuery(context);

        var kpis = await BuildDashboardKpisAsync(profileQuery, assignmentsQuery, context, ct);
        var jobKpis = await BuildJobKpisAsync(jobsQuery, ct);
        var invitationKpis = await BuildInvitationKpisAsync(jobsQuery, ct);

        return Result.Ok(new DashboardOverviewDto
        {
            Role = context.CanViewAllProfiles ? "HrManager" : context.CanReviewProfiles ? "DepartmentManager" : "Employee",
            Filters = BuildFilters(request),
            Kpis = kpis,
            JobKpis = jobKpis,
            InvitationKpis = invitationKpis
        });
    }

    public async Task<Result<CandidateStatusSummaryDto>> GetCandidateStatusAsync(
        DashboardQueryBase request,
        CancellationToken ct)
    {
        var contextResult = await CreateContextAsync(request, ct);
        if (contextResult.IsFailed) return Result.Fail(contextResult.Errors);

        var profileQuery = BuildProfilesQuery(request, contextResult.Value);
        var totalProfiles = await profileQuery.CountAsync(ct);
        var byStatus = await GetProfilesByStatusAsync(profileQuery, ct);

        return Result.Ok(new CandidateStatusSummaryDto
        {
            TotalProfiles = totalProfiles,
            InCreationProfiles = await profileQuery.CountAsync(x => x.Status == UserProfileStatus.InCreation, ct),
            SubmittedProfiles = await profileQuery.CountAsync(x => x.Status == UserProfileStatus.Submitted, ct),
            UnderReviewProfiles = await profileQuery.CountAsync(x => x.Status == UserProfileStatus.UnderReview, ct),
            ApprovedProfiles = await profileQuery.CountAsync(x => x.Status == UserProfileStatus.Approved, ct),
            ReturnedProfiles = await profileQuery.CountAsync(x => x.Status == UserProfileStatus.RequiresUpdate, ct),
            RejectedProfiles = await CountRejectedProfilesAsync(contextResult.Value.Range.From, contextResult.Value.Range.To, ct),
            ProfileBreakdown = new ProfileBreakdownDto
            {
                ByStatus = byStatus,
                ByDepartment = await GetProfilesByTargetEntityAsync(profileQuery, ct),
                ByPriority = await GetProfilesByPriorityAsync(profileQuery, ct),
                ByCandidateType = await GetProfilesByCandidateTypeAsync(profileQuery, ct),
                Aging = await GetProfilesAgingAsync(profileQuery, DateTime.UtcNow, ct)
            }
        });
    }

    public async Task<Result<CandidateTypeSummaryDto>> GetCandidateTypesAsync(
        DashboardQueryBase request,
        CancellationToken ct)
    {
        var contextResult = await CreateContextAsync(request, ct);
        if (contextResult.IsFailed) return Result.Fail(contextResult.Errors);

        var byCandidateType = await GetProfilesByCandidateTypeAsync(BuildProfilesQuery(request, contextResult.Value), ct);

        return Result.Ok(new CandidateTypeSummaryDto
        {
            CandidateTypeKpis = BuildCandidateTypeKpis(byCandidateType),
            ByCandidateType = byCandidateType
        });
    }

    public async Task<Result<JobsSummaryDto>> GetJobsSummaryAsync(DashboardQueryBase request, CancellationToken ct)
    {
        var contextResult = await CreateContextAsync(request, ct);
        if (contextResult.IsFailed) return Result.Fail(contextResult.Errors);

        var jobsQuery = BuildJobsQuery(request, contextResult.Value);

        return Result.Ok(new JobsSummaryDto
        {
            JobKpis = await BuildJobKpisAsync(jobsQuery, ct),
            InvitationKpis = await BuildInvitationKpisAsync(jobsQuery, ct),
            JobBreakdown = new JobBreakdownDto
            {
                ByStatus = await jobsQuery
                    .GroupBy(x => x.JobStatus != null ? x.JobStatus.BackendName : "N/A")
                    .Select(g => new StatusCountDto { Status = g.Key, Count = g.Count() })
                    .ToListAsync(ct),
                ByDepartment = await jobsQuery
                    .GroupBy(x => x.Management != null ? x.Management.NameEn : "N/A")
                    .Select(g => new GroupCountDto { Label = Truncate(g.Key), Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(MaxTopItems)
                    .ToListAsync(ct)
            }
        });
    }

    public async Task<Result<IReadOnlyList<LatestJobDto>>> GetLatestJobsAsync(
        DashboardQueryBase request,
        CancellationToken ct)
    {
        var contextResult = await CreateContextAsync(request, ct);
        if (contextResult.IsFailed) return Result.Fail(contextResult.Errors);

        var jobsQuery = BuildJobsQuery(request, contextResult.Value);
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var invitationRepo = uow.GetEntityRepository<Invitation>();
        var activeInvitationStatuses = new[]
        {
            InvitationStatusIds.NewInvitation,
            InvitationStatusIds.Read,
            InvitationStatusIds.PendingAttachmentApproval,
            InvitationStatusIds.ReturnedAttachment,
            InvitationStatusIds.ExamEligible,
            InvitationStatusIds.Rejected
        };

        var raw = await jobsQuery
            .OrderByDescending(x => x.UpdatedDate)
            .ThenByDescending(x => x.CreatedDate)
            .Take(4)
            .Select(job => new
            {
                job.Id,
                JobTitleAr = job.JobTitle != null ? job.JobTitle.JobNameAr : string.Empty,
                JobTitleEn = job.JobTitle != null ? job.JobTitle.JobNameEn : string.Empty,
                job.Management,
                Status = job.JobStatus != null ? job.JobStatus.BackendName : "N/A",
                CandidatesCount = profileRepo.DbSet.Count(profile =>
                    !profile.IsDeleted &&
                    profile.Status == UserProfileStatus.Approved &&
                    profile.AvailableForRecruitment &&
                    profile.TargetEntityId == job.WorkLocationId &&
                    (job.GenderId == null || job.GenderId == GenderIds.All || profile.GenderId == job.GenderId) &&
                    !invitationRepo.DbSet.Any(invitation =>
                        !invitation.IsDeleted &&
                        invitation.JobId == job.Id &&
                        invitation.ApplicantId == profile.UserId &&
                        activeInvitationStatuses.Contains(invitation.InvitationStatusId))),
                InvitationsSent = invitationRepo.DbSet.Count(invitation => !invitation.IsDeleted && invitation.JobId == job.Id)
            })
            .ToListAsync(ct);

        return Result.Ok<IReadOnlyList<LatestJobDto>>(raw.Select(job => new LatestJobDto
        {
            JobId = job.Id,
            JobTitle = localizationService.GetLocalizedValue(job.JobTitleAr, job.JobTitleEn),
            ManagementName = localizationService.GetLocalizedName(job.Management),
            Status = job.Status,
            CandidatesCount = job.CandidatesCount,
            InvitationsSent = job.InvitationsSent
        }).ToList());
    }

    public async Task<Result<EmployeeIndicatorsDto>> GetEmployeeIndicatorsAsync(
        DashboardQueryBase request,
        CancellationToken ct)
    {
        var contextResult = await CreateContextAsync(request, ct);
        if (contextResult.IsFailed) return Result.Fail(contextResult.Errors);

        var context = contextResult.Value;
        var profileQuery = BuildProfilesQuery(request, context);
        var taskAgg = await BuildTaskAggregateAsync(BuildAssignmentsQuery(context), ct);

        return Result.Ok(new EmployeeIndicatorsDto
        {
            ActiveEmployees = context.ActiveEmployees,
            RemainingTasks = taskAgg.Remaining,
            UnassignedProfiles = await CountUnassignedProfilesAsync(profileQuery, ct),
            CompletedTasks = taskAgg.Completed
        });
    }

    public async Task<Result<EmployeeReviewOutcomesDto>> GetEmployeeReviewOutcomesAsync(
        DashboardQueryBase request, CancellationToken ct)
    {
        var contextResult = await CreateContextAsync(request, ct);
        if (contextResult.IsFailed) return Result.Fail(contextResult.Errors);

        var profileQuery = BuildProfilesQuery(request, contextResult.Value);

        return Result.Ok(new EmployeeReviewOutcomesDto
        {
            TotalProfiles = await profileQuery.CountAsync(ct),
            ApprovedProfiles = await profileQuery.CountAsync(x => x.Status == UserProfileStatus.Approved, ct),
            ReturnedProfiles = await profileQuery.CountAsync(x => x.Status == UserProfileStatus.RequiresUpdate, ct),
            UnassignedProfiles = await CountUnassignedProfilesAsync(profileQuery, ct),
            PendingProfiles = await profileQuery.CountAsync(x => x.Status == UserProfileStatus.UnderReview, ct)
        });
    }

    public async Task<Result<PaginatedResult<TeamPerformanceRowDto>>> GetTeamPerformanceAsync(
        GetTeamPerformanceQuery request,
        CancellationToken ct)
    {
        var userIdText = currentUserService.UserId;
        if (string.IsNullOrWhiteSpace(userIdText) || !Guid.TryParse(userIdText, out var currentUserId))
            return Result.Fail("Unauthorized");

        var projection = new ProfileDistributionProjection(uow, userManager, userRepository, localizationService, mapper);
        var employees = await projection.LoadEmployeesAsync(currentUserId, ct);
        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();
        var assignedEmployeeIds = await assignmentRepo.DbSet
            .AsNoTracking()
            .Where(p => p.UserProfile != null &&
                        (p.UserProfile.Provider == nameof(ProviderLoginIds.QatarResidentOtp) ||
                         p.UserProfile.Provider == nameof(ProviderLoginIds.QatarPass)))
            .Select(x => x.EmployeeId)
            .Distinct()
            .ToListAsync(ct);

        var employeeIds = employees
            .Select(x => x.EmployeeId)
            .Union(assignedEmployeeIds)
            .ToList();

        var employeeList = await userManager.Users
            .OfType<EmployeeUser>()
            .AsNoTracking()
            .Include(x => x.EmployeeProfile)
            .Where(x => employeeIds.Contains(x.Id))
            .ToListAsync(ct);

        // var allowedCountryId = await ResolveAllowedCountryIdAsync(currentUserId, ct);
        var assignmentsQuery = assignmentRepo.DbSet
            .AsNoTracking()
            .Include(x => x.UserProfile)
            //.WhereIf(allowedCountryId is not null, p => p.UserProfile!.ResidenceCountryId == allowedCountryId)
            .Where(p=> p.UserProfile!.Provider == nameof(ProviderLoginIds.QatarResidentOtp) ||
                       p.UserProfile!.Provider == nameof(ProviderLoginIds.QatarPass))
            .Where(x => employeeIds.Contains(x.EmployeeId));

        if (request.FromDateUtc.HasValue)
            assignmentsQuery = assignmentsQuery.Where(x => x.AssignedAtUtc >= request.FromDateUtc.Value);
        if (request.ToDateUtc.HasValue)
            assignmentsQuery = assignmentsQuery.Where(x => x.AssignedAtUtc <= request.ToDateUtc.Value);
        if (request.EmployeeId.HasValue)
            assignmentsQuery = assignmentsQuery.Where(x => x.EmployeeId == request.EmployeeId.Value);

        var overdueCutoff = DateTimeOffset.UtcNow.AddDays(-OverdueAfterDays);
        var assignmentStats = await GetTeamAssignmentStatsAsync(assignmentsQuery, overdueCutoff, ct);
        var reviewStats = await GetTeamReviewStatsAsync(uow.GetEntityRepository<ReviewItem>(), employeeIds, ct);
        var totalAssignedTasks = await assignmentsQuery.CountAsync(x => x.IsActive, ct);

        var allRows = BuildTeamRows(employeeList, assignmentStats, reviewStats, totalAssignedTasks, localizationService);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.ToLowerInvariant();
            allRows = allRows.Where(x =>
                x.Name.ToLowerInvariant().Contains(searchTerm) ||
                (x.EmployeeNumber != null && x.EmployeeNumber.ToLowerInvariant().Contains(searchTerm)) ||
                (x.DepartmentName != null && x.DepartmentName.ToLowerInvariant().Contains(searchTerm)) ||
                (x.JobDescription != null && x.JobDescription.ToLowerInvariant().Contains(searchTerm))
            ).ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            var isDesc = string.Equals(request.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
            allRows = request.SortBy switch
            {
                nameof(TeamPerformanceRowDto.Name) => isDesc ? allRows.OrderByDescending(x => x.Name).ToList() : allRows.OrderBy(x => x.Name).ToList(),
                nameof(TeamPerformanceRowDto.EmployeeNumber) => isDesc ? allRows.OrderByDescending(x => x.EmployeeNumber).ToList() : allRows.OrderBy(x => x.EmployeeNumber).ToList(),
                nameof(TeamPerformanceRowDto.AssignedTasks) => isDesc ? allRows.OrderByDescending(x => x.AssignedTasks).ToList() : allRows.OrderBy(x => x.AssignedTasks).ToList(),
                nameof(TeamPerformanceRowDto.CompletedTasks) => isDesc ? allRows.OrderByDescending(x => x.CompletedTasks).ToList() : allRows.OrderBy(x => x.CompletedTasks).ToList(),
                nameof(TeamPerformanceRowDto.OverdueTasks) => isDesc ? allRows.OrderByDescending(x => x.OverdueTasks).ToList() : allRows.OrderBy(x => x.OverdueTasks).ToList(),
                nameof(TeamPerformanceRowDto.ApprovalRate) => isDesc ? allRows.OrderByDescending(x => x.ApprovalRate).ToList() : allRows.OrderBy(x => x.ApprovalRate).ToList(),
                nameof(TeamPerformanceRowDto.AverageResponseHours) => isDesc ? allRows.OrderByDescending(x => x.AverageResponseHours).ToList() : allRows.OrderBy(x => x.AverageResponseHours).ToList(),
                nameof(TeamPerformanceRowDto.ProfilesReviewed) => isDesc ? allRows.OrderByDescending(x => x.ProfilesReviewed).ToList() : allRows.OrderBy(x => x.ProfilesReviewed).ToList(),
                nameof(TeamPerformanceRowDto.ProductivityScore) => isDesc ? allRows.OrderByDescending(x => x.ProductivityScore).ToList() : allRows.OrderBy(x => x.ProductivityScore).ToList(),
                _ => allRows
            };
        }

        var total = allRows.Count;
        var rows = allRows
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return Result.Ok(new PaginatedResult<TeamPerformanceRowDto>(rows, total, request.PageNumber, request.PageSize));
    }

    private async Task<DashboardQueryContext> BuildContextAsync(DashboardQueryBase request, CancellationToken ct)
    {
        var userIdText = currentUserService.UserId;
        if (string.IsNullOrWhiteSpace(userIdText) || !Guid.TryParse(userIdText, out var userId))
            throw new UnauthorizedAccessException();

        var roles = await userManager.GetRolesAsync(new User { Id = userId });
        var permissions = await GetPermissionsAsync(roles, ct);
        var employees = await userManager.Users
            .OfType<EmployeeUser>()
            .AsNoTracking()
            .Include(x => x.EmployeeProfile)
            .ToListAsync(ct);

        return new DashboardQueryContext(
            userId,
            employees.Select(x => x.Id).ToList(),
            employees.Count(x => !x.IsBlocked),
            permissions.Contains(PermissionKeys.ProfileDistribution.View),
            permissions.Contains(PermissionKeys.ProfileApproval.Review),
            permissions.Contains(PermissionKeys.MinisterOffice.View),
            null, //await ResolveAllowedCountryIdAsync(userId, ct),
            ResolveRange(request, DateTime.UtcNow));
    }

    private async Task<Result<DashboardQueryContext>> CreateContextAsync(DashboardQueryBase request, CancellationToken ct)
    {
        try
        {
            return Result.Ok(await BuildContextAsync(request, ct));
        }
        catch (UnauthorizedAccessException)
        {
            return Result.Fail("Unauthorized");
        }
    }

    private IQueryable<UserProfile> BuildProfilesQuery(DashboardQueryBase request, DashboardQueryContext context)
    {
        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();
        var query = uow.GetEntityRepository<UserProfile>().DbSet
            .AsNoTracking()
            .Include(x => x.TargetEntity)
            .Include(x => x.CandidateType)
            //.WhereIf(context.AllowedCountryId is not null, x => x.ResidenceCountryId == context.AllowedCountryId)
            .Where(p=> p.Provider == nameof(ProviderLoginIds.QatarResidentOtp) ||
                       p.Provider == nameof(ProviderLoginIds.QatarPass))
            .Where(x => !x.IsDeleted);

        if (request.FromDateUtc.HasValue) query = query.Where(x => x.CreatedDate >= request.FromDateUtc.Value);
        if (request.ToDateUtc.HasValue) query = query.Where(x => x.CreatedDate <= request.ToDateUtc.Value);
        if (request.DepartmentId.HasValue) query = query.Where(x => x.TargetEntityId == request.DepartmentId);

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<UserProfileStatus>(request.Status, true, out var status))
        {
            query = query.Where(x => x.Status == status);
        }

        if (!context.CanViewAllProfiles)
        {
            query = query.Where(x => assignmentRepo.DbSet.Any(a =>
                a.UserProfileId == x.Id && context.EmployeeIds.Contains(a.EmployeeId)));
        }
        else if (request.EmployeeId.HasValue)
        {
            var employeeId = request.EmployeeId.Value;
            query = query.Where(x => assignmentRepo.DbSet.Any(a => a.UserProfileId == x.Id && a.EmployeeId == employeeId));
        }

        return query;
    }

    private IQueryable<Job> BuildJobsQuery(DashboardQueryBase request, DashboardQueryContext context)
    {
        var query = uow.GetEntityRepository<Job>().DbSet
            .AsNoTracking()
            .Include(x => x.JobStatus)
            .Include(x => x.JobTitle)
            .Include(x => x.Management)
            .Where(x => !x.IsDeleted);

        if (request.FromDateUtc.HasValue) query = query.Where(x => x.CreatedDate >= request.FromDateUtc.Value);
        if (request.ToDateUtc.HasValue) query = query.Where(x => x.CreatedDate <= request.ToDateUtc.Value);
        if (request.DepartmentId.HasValue) query = query.Where(x => x.DepartmentId == request.DepartmentId);

        return query;
    }

    private IQueryable<ProfileAssignment> BuildAssignmentsQuery(DashboardQueryContext context) =>
        uow.GetEntityRepository<ProfileAssignment>().DbSet
            .AsNoTracking()
            .Include(x => x.UserProfile)
            //.WhereIf(context.AllowedCountryId is not null, x => x.UserProfile!.ResidenceCountryId == context.AllowedCountryId)
            .Where(p=> p.UserProfile!.Provider == nameof(ProviderLoginIds.QatarResidentOtp) ||
                       p.UserProfile!.Provider == nameof(ProviderLoginIds.QatarPass))
            .Where(x => !x.IsDeleted && context.EmployeeIds.Contains(x.EmployeeId));

    private async Task<DashboardKpisDto> BuildDashboardKpisAsync(
        IQueryable<UserProfile> profileQuery,
        IQueryable<ProfileAssignment> assignmentsQuery,
        DashboardQueryContext context,
        CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var totalProfiles = await profileQuery.CountAsync(ct);
        var approvedProfiles = await profileQuery.CountAsync(x => x.Status == UserProfileStatus.Approved, ct);
        var rejectedProfiles = await CountRejectedProfilesAsync(context.Range.From, context.Range.To, ct);
        var taskAgg = await BuildTaskAggregateAsync(assignmentsQuery, ct);

        return new DashboardKpisDto
        {
            TotalEmployees = context.EmployeeIds.Count,
            ActiveEmployees = context.ActiveEmployees,
            TotalProfiles = totalProfiles,
            NewProfilesToday = await CountNewProfilesAsync(now.Date, now, ct),
            NewProfilesThisWeek = await CountNewProfilesAsync(GetWeekStart(now.Date), now, ct),
            NewProfilesThisMonth = await CountNewProfilesAsync(new DateTime(now.Year, now.Month, 1), now, ct),
            ApprovedProfiles = approvedProfiles,
            RejectedProfiles = rejectedProfiles,
            InCreationProfiles = await profileQuery.CountAsync(x => x.Status == UserProfileStatus.InCreation, ct),
            SubmittedProfiles = await profileQuery.CountAsync(x => x.Status == UserProfileStatus.Submitted, ct),
            UnderReviewProfiles = await profileQuery.CountAsync(x => x.Status == UserProfileStatus.UnderReview, ct),
            PendingProfiles = await profileQuery.CountAsync(x =>
                x.Status == UserProfileStatus.Submitted || x.Status == UserProfileStatus.UnderReview, ct),
            ReturnedProfiles = await profileQuery.CountAsync(x => x.Status == UserProfileStatus.RequiresUpdate, ct),
            ApprovalRate = totalProfiles == 0 ? 0 : Math.Round(approvedProfiles * 100m / totalProfiles, 2),
            RejectionRate = totalProfiles == 0 ? 0 : Math.Round(rejectedProfiles * 100m / totalProfiles, 2),
            AverageApprovalHours = await CalculateAverageApprovalHoursAsync(context.Range.From, context.Range.To, ct),
            TotalAssignedTasks = taskAgg.Total,
            CompletedTasks = taskAgg.Completed,
            RemainingTasks = taskAgg.Remaining,
            OverdueTasks = taskAgg.Overdue,
            UnassignedProfiles = await CountUnassignedProfilesAsync(profileQuery, ct),
            FollowedMinisterOfficeCandidates = context.CanViewMinisterOffice
                ? await uow.GetEntityRepository<Tawtheef.Domain.Entities.MinisterOffice.MinisterOfficeCandidate>().DbSet
                    .CountAsync(x => !x.IsDeleted && x.IsFollowUpActive, ct)
                : 0
        };
    }

    private async Task<JobKpisDto> BuildJobKpisAsync(IQueryable<Job> jobsQuery, CancellationToken ct) =>
        new()
        {
            TotalJobs = await jobsQuery.CountAsync(ct),
            DraftJobs = await jobsQuery.CountAsync(x => x.JobStatusId == JobStatusIds.Draft, ct),
            ActiveJobs = await jobsQuery.CountAsync(x => x.JobStatusId == JobStatusIds.Published, ct),
            PendingReviewJobs = await jobsQuery.CountAsync(x => x.JobStatusId == JobStatusIds.PendingApproval, ct),
            ApprovedJobs = await jobsQuery.CountAsync(x =>
                x.JobStatusId == JobStatusIds.PendingPointConfiguration ||
                x.JobStatusId == JobStatusIds.NeedPointUpdate ||
                x.JobStatusId == JobStatusIds.PendingPointApproval, ct),
            RejectedJobs = await jobsQuery.CountAsync(x => x.JobStatusId == JobStatusIds.Rejected, ct),
            NewJobsToday = await uow.GetEntityRepository<Job>().DbSet.CountAsync(x => !x.IsDeleted && x.CreatedDate >= DateTime.UtcNow.Date, ct),
            PendingPointConfigurationJobs = await jobsQuery.CountAsync(x => x.JobStatusId == JobStatusIds.PendingPointConfiguration, ct),
            NeedPointUpdateJobs = await jobsQuery.CountAsync(x => x.JobStatusId == JobStatusIds.NeedPointUpdate, ct),
            PendingPointApprovalJobs = await jobsQuery.CountAsync(x => x.JobStatusId == JobStatusIds.PendingPointApproval, ct),
            NeedUpdateJobs = await jobsQuery.CountAsync(x => x.JobStatusId == JobStatusIds.NeedUpdate, ct),
            ReadyForAnnouncementJobs = await jobsQuery.CountAsync(x => x.JobStatusId == JobStatusIds.ReadyForAnnouncement, ct),
            PublishedJobs = await jobsQuery.CountAsync(x => x.JobStatusId == JobStatusIds.Published, ct),
            ClosedJobs = await jobsQuery.CountAsync(x => x.JobStatusId == JobStatusIds.Closed, ct),
            CancelledJobs = await jobsQuery.CountAsync(x => x.JobStatusId == JobStatusIds.Cancelled, ct)
        };

    private async Task<InvitationKpisDto> BuildInvitationKpisAsync(IQueryable<Job> jobsQuery, CancellationToken ct)
    {
        var jobIds = jobsQuery.Select(x => x.Id);
        var invitationQuery = uow.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .Where(x => !x.IsDeleted && jobIds.Contains(x.JobId));

        return new InvitationKpisDto
        {
            TotalInvitations = await invitationQuery.CountAsync(ct),
            AcceptedInvitations = await invitationQuery.CountAsync(x => x.IsAccepted, ct),
            PendingInvitations = await invitationQuery.CountAsync(x =>
                !x.IsAccepted &&
                x.InvitationStatusId != InvitationStatusIds.Rejected &&
                x.InvitationStatusId != InvitationStatusIds.Closed &&
                x.InvitationStatusId != InvitationStatusIds.Cancelled, ct),
            PendingAttachmentApproval = await invitationQuery.CountAsync(x =>
                x.InvitationStatusId == InvitationStatusIds.PendingAttachmentApproval, ct)
        };
    }

    private async Task<(int Total, int Completed, int Remaining, int Overdue)> BuildTaskAggregateAsync(
        IQueryable<ProfileAssignment> assignmentsQuery,
        CancellationToken ct)
    {
        var overdueCutoff = DateTimeOffset.UtcNow.AddDays(-OverdueAfterDays);
        var row = await assignmentsQuery
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Count(x => x.UserProfile == null || x.UserProfile.Status != UserProfileStatus.UnderReview || x.IsActive),
                Completed = g.Count(x =>
                    x.UnassignedAtUtc != null ||
                    x.UserProfile!.Status == UserProfileStatus.Approved ||
                    x.UserProfile.Status == UserProfileStatus.RequiresUpdate),
                Remaining = g.Count(x =>
                    x.IsActive &&
                    (x.UserProfile!.Status == UserProfileStatus.UnderReview || x.UserProfile.Status == UserProfileStatus.Submitted)),
                Overdue = g.Count(x =>
                    x.IsActive &&
                    x.UnassignedAtUtc == null &&
                    x.AssignedAtUtc <= overdueCutoff &&
                    x.UserProfile!.Status == UserProfileStatus.UnderReview)
            })
            .OrderBy(g=> g.Total)
            .FirstOrDefaultAsync(ct);

        return (row?.Total ?? 0, row?.Completed ?? 0, row?.Remaining ?? 0, row?.Overdue ?? 0);
    }

    private Task<int> CountUnassignedProfilesAsync(IQueryable<UserProfile> profileQuery, CancellationToken ct)
    {
        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();
        return profileQuery.CountAsync(p => p.Status == UserProfileStatus.Submitted &&
            !assignmentRepo.DbSet.Any(a => 
                a.UserProfileId == p.Id && a.IsActive && a.UnassignedAtUtc == null), ct);
    }

    private async Task<List<string>> GetPermissionsAsync(IList<string> roles, CancellationToken ct)
    {
        if (roles.Count == 0) return [];

        var query = from rc in uow.Context.Set<IdentityRoleClaim<Guid>>().AsNoTracking()
            join r in uow.Context.Set<ApplicationRole>().AsNoTracking() on rc.RoleId equals r.Id
            where roles.Contains(r.Name!) && rc.ClaimType == "permission"
            select rc.ClaimValue;

        return await query.Where(x => x != null).Select(x => x!).Distinct().ToListAsync(ct);
    }

    private Task<int> CountRejectedProfilesAsync(DateTime from, DateTime to, CancellationToken ct) =>
        uow.GetEntityRepository<ReviewItem>().DbSet
            .AsNoTracking()
            .Where(x =>
                x.Status == ReviewStatus.Rejected &&
                x.UserProfile != null &&
                x.UserProfile.CreatedDate >= from &&
                x.UserProfile.CreatedDate <= to)
            .Select(x => x.UserProfileId)
            .Distinct()
            .CountAsync(ct);

    private Task<int> CountNewProfilesAsync(DateTime from, DateTime to, CancellationToken ct) =>
        uow.GetEntityRepository<UserProfile>().DbSet.CountAsync(x => !x.IsDeleted && x.CreatedDate >= from && x.CreatedDate <= to, ct);

    private async Task<decimal> CalculateAverageApprovalHoursAsync(DateTime from, DateTime to, CancellationToken ct)
    {
        var approvalsQuery = uow.GetEntityRepository<UserProfile>().DbSet
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                x.CreatedDate >= from &&
                x.CreatedDate <= to &&
                x.Status == UserProfileStatus.Approved &&
                x.UpdatedDate != null);

        var count = await approvalsQuery.CountAsync(ct);
        if (count == 0) return 0m;

        var totalHours = await approvalsQuery
            .Select(x => EF.Functions.DateDiffSecond(x.CreatedDate, x.UpdatedDate!.Value) / 3600.0)
            .SumAsync(ct);

        return Math.Round((decimal)(totalHours / count), 2);
    }

    private static async Task<List<StatusCountDto>> GetProfilesByStatusAsync(IQueryable<UserProfile> profileQuery, CancellationToken ct)
    {
        var raw = await profileQuery
            .GroupBy(x => x.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        return raw.Select(x => new StatusCountDto { Status = x.Status.ToString(), Count = x.Count }).ToList();
    }

    private static Task<List<GroupCountDto>> GetProfilesByTargetEntityAsync(IQueryable<UserProfile> profileQuery, CancellationToken ct) =>
        profileQuery
            .GroupBy(x => x.TargetEntity != null ? x.TargetEntity.NameEn : "N/A")
            .Select(g => new GroupCountDto { Label = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(MaxTopItems)
            .ToListAsync(ct);

    private static Task<List<GroupCountDto>> GetProfilesByPriorityAsync(IQueryable<UserProfile> profileQuery, CancellationToken ct) =>
        profileQuery
            .Select(x => new
            {
                Priority = x.Status == UserProfileStatus.Submitted ? "high"
                    : x.Status == UserProfileStatus.UnderReview ? "medium"
                    : "low"
            })
            .GroupBy(x => x.Priority)
            .Select(g => new GroupCountDto { Label = g.Key, Count = g.Count() })
            .ToListAsync(ct);

    private static Task<List<AgingBucketDto>> GetProfilesAgingAsync(IQueryable<UserProfile> profileQuery, DateTime now, CancellationToken ct) =>
        profileQuery
            .Select(x => new { Days = EF.Functions.DateDiffDay(x.CreatedDate, now) })
            .GroupBy(x => x.Days <= 1 ? "0-1d" : x.Days <= 3 ? "2-3d" : x.Days <= 7 ? "4-7d" : ">7d")
            .Select(g => new AgingBucketDto { Bucket = g.Key, Count = g.Count() })
            .ToListAsync(ct);

    private static async Task<List<CandidateTypeCountDto>> GetProfilesByCandidateTypeAsync(
        IQueryable<UserProfile> profileQuery,
        CancellationToken ct)
    {
        var knownTypes = new[]
        {
            new { Id = CandidateTypeIds.Qatari, Key = "qatari", Label = "Qatari" },
            new { Id = CandidateTypeIds.NonQatari, Key = "nonQatari", Label = "Non Qatari" },
            new { Id = CandidateTypeIds.SonOfQatariMother, Key = "sonOfQatariMother", Label = "Son of Qatari Mother" },
            new { Id = CandidateTypeIds.WifeOfQatari, Key = "wifeOfQatari", Label = "Wife of Qatari" },
            new { Id = CandidateTypeIds.GCC, Key = "gcc", Label = "GCC" },
            new { Id = CandidateTypeIds.ResidentQatar, Key = "residentQatar", Label = "Resident Qatar" }
        };

        var raw = await profileQuery
            .Where(p=> p.Status == UserProfileStatus.Approved)
            .GroupBy(x => new { x.CandidateTypeId, Label = x.CandidateType != null ? x.CandidateType.NameEn : "N/A" })
            .Select(g => new { g.Key.CandidateTypeId, g.Key.Label, Count = g.Count() })
            .ToListAsync(ct);

        var rows = knownTypes.Select(type =>
        {
            var item = raw.FirstOrDefault(x => x.CandidateTypeId == type.Id);
            return new CandidateTypeCountDto
            {
                CandidateTypeId = type.Id,
                Key = type.Key,
                Label = item?.Label ?? type.Label,
                Count = item?.Count ?? 0
            };
        }).ToList();

        rows.AddRange(raw
            .Where(x => x.CandidateTypeId is null || knownTypes.All(type => type.Id != x.CandidateTypeId))
            .Select(x => new CandidateTypeCountDto
            {
                CandidateTypeId = x.CandidateTypeId,
                Key = GetCandidateTypeKey(x.CandidateTypeId),
                Label = x.Label,
                Count = x.Count
            }));

        return rows;
    }

    private static CandidateTypeKpisDto BuildCandidateTypeKpis(IReadOnlyCollection<CandidateTypeCountDto> byCandidateType)
    {
        var counts = byCandidateType
            .GroupBy(x => x.Key)
            .ToDictionary(x => x.Key, x => x.Sum(v => v.Count), StringComparer.OrdinalIgnoreCase);

        return new CandidateTypeKpisDto
        {
            Total = byCandidateType.Sum(x => x.Count),
            Qatari = GetCandidateTypeCount(counts, "qatari"),
            NonQatari = GetCandidateTypeCount(counts, "nonQatari"),
            SonOfQatariMother = GetCandidateTypeCount(counts, "sonOfQatariMother"),
            WifeOfQatari = GetCandidateTypeCount(counts, "wifeOfQatari"),
            Gcc = GetCandidateTypeCount(counts, "gcc"),
            ResidentQatar = GetCandidateTypeCount(counts, "residentQatar"),
            Unknown = GetCandidateTypeCount(counts, "unknown")
        };
    }

    private static int GetCandidateTypeCount(IReadOnlyDictionary<string, int> counts, string key) =>
        counts.TryGetValue(key, out var count) ? count : 0;

    private static string GetCandidateTypeKey(Guid? candidateTypeId)
    {
        if (candidateTypeId == CandidateTypeIds.Qatari) return "qatari";
        if (candidateTypeId == CandidateTypeIds.NonQatari) return "nonQatari";
        if (candidateTypeId == CandidateTypeIds.SonOfQatariMother) return "sonOfQatariMother";
        if (candidateTypeId == CandidateTypeIds.WifeOfQatari) return "wifeOfQatari";
        if (candidateTypeId == CandidateTypeIds.GCC) return "gcc";
        if (candidateTypeId == CandidateTypeIds.ResidentQatar) return "residentQatar";
        return "unknown";
    }

    private static DashboardFiltersSnapshotDto BuildFilters(DashboardQueryBase request) =>
        new()
        {
            FromDateUtc = request.FromDateUtc,
            ToDateUtc = request.ToDateUtc,
            DepartmentId = request.DepartmentId,
            EmployeeId = request.EmployeeId,
            Status = request.Status
        };

    private static (DateTime From, DateTime To) ResolveRange(DashboardQueryBase request, DateTime nowUtc)
    {
        var from = request.FromDateUtc ?? nowUtc.AddDays(-DefaultLookbackDays);
        var to = request.ToDateUtc ?? nowUtc;
        if (from > to) (from, to) = (to, from);
        return (from, to);
    }

    private static DateTime GetWeekStart(DateTime todayStartUtc) =>
        todayStartUtc.AddDays(-(int)todayStartUtc.DayOfWeek);

    private static string Truncate(string value, int maxLength = 20) =>
        string.IsNullOrEmpty(value) || value.Length <= maxLength ? value : value[..maxLength] + "...";

    private static List<TeamPerformanceRowDto> BuildTeamRows(
        IReadOnlyCollection<EmployeeUser> employees,
        IReadOnlyDictionary<Guid, TeamAssignmentStat> assignmentLookup,
        IReadOnlyDictionary<Guid, TeamReviewStat> reviewLookup,
        int globalTotalAssigned,
        ILocalizationService localizationService)
    {
        return employees.Select(employee =>
        {
            assignmentLookup.TryGetValue(employee.Id, out var assign);
            reviewLookup.TryGetValue(employee.Id, out var review);

            var assigned = assign?.Assigned ?? 0;
            var completed = assign?.Completed ?? 0;
            var active = assign?.Active ?? 0;
            var overdue = assign?.Overdue ?? 0;
            var reviewed = review?.Reviewed ?? 0;
            var approved = review?.Approved ?? 0;
            var rejectionRate = reviewed == 0 ? 0 : Math.Round((reviewed - approved) * 100m / reviewed, 2);
            var approvalRate = reviewed == 0 ? 0 : Math.Round(approved * 100m / reviewed, 2);
            var workloadRatio = globalTotalAssigned == 0 ? 0 : Math.Round(assigned * 100m / globalTotalAssigned, 2);
            var productivityScore = Math.Max(0, (completed * 2) + approved - overdue);

            return new TeamPerformanceRowDto
            {
                EmployeeId = employee.Id,
                Name = localizationService.GetLocalizedFullName(employee),
                EmployeeNumber = employee.EmployeeProfile?.EmployeeNumber,
                DepartmentName = employee.EmployeeProfile?.Department,
                JobDescription = employee.EmployeeProfile?.JobTitle,
                AssignedTasks = assigned,
                ActiveTasks = active,
                CompletedTasks = completed,
                RemainingTasks = assign?.Remaining ?? 0,
                OverdueTasks = overdue,
                ProfilesReviewed = reviewed,
                ApprovalRate = approvalRate,
                RejectionRate = rejectionRate,
                WorkloadRatio = workloadRatio,
                WorkloadBalanceIndicator = workloadRatio > 20 ? "High" : workloadRatio > 10 ? "Balanced" : "Low",
                ProductivityScore = productivityScore
            };
        }).ToList();
    }

    private static async Task<Dictionary<Guid, TeamAssignmentStat>> GetTeamAssignmentStatsAsync(
        IQueryable<ProfileAssignment> assignmentsQuery,
        DateTimeOffset overdueCutoff,
        CancellationToken ct)
    {
        var raw = await assignmentsQuery
            .GroupBy(x => x.EmployeeId)
            .Select(g => new TeamAssignmentStat(
                g.Key,
                g.Count(x => x.IsActive),
                g.Count(x => x.IsActive),
                g.Count(x => x.UserProfile != null &&
                             (x.UserProfile.Status == UserProfileStatus.Approved ||
                              x.UserProfile.Status == UserProfileStatus.RequiresUpdate)),
                g.Count(x => x.IsActive &&
                             (x.UserProfile == null || x.UserProfile.Status == UserProfileStatus.UnderReview)),
                g.Count(x =>
                    x.IsActive &&
                    x.UserProfile != null &&
                    (x.UserProfile.Status == UserProfileStatus.UnderReview ||
                     x.UserProfile.Status == UserProfileStatus.RequiresUpdate) &&
                    x.AssignedAtUtc <= overdueCutoff)))
            .ToListAsync(ct);

        return raw.ToDictionary(x => x.EmployeeId);
    }

    private static async Task<Dictionary<Guid, TeamReviewStat>> GetTeamReviewStatsAsync(
        IGenericRepository<ReviewItem> reviewRepo,
        IReadOnlyCollection<Guid> employeeIds,
        CancellationToken ct)
    {
        var raw = await reviewRepo.DbSet
            .AsNoTracking()
            .Where(x => x.ReviewedById != null && employeeIds.Contains(x.ReviewedById.Value))
            .GroupBy(x => x.ReviewedById!.Value)
            .Select(g => new TeamReviewStat(
                g.Key,
                g.Select(x => x.UserProfileId).Distinct().Count(),
                g.Where(x => x.Status == ReviewStatus.Approved).Select(x => x.UserProfileId).Distinct().Count(),
                g.Where(x => x.Status == ReviewStatus.Rejected).Select(x => x.UserProfileId).Distinct().Count()))
            .ToListAsync(ct);

        return raw.ToDictionary(x => x.EmployeeId);
    }

    private sealed record DashboardQueryContext(
        Guid CurrentUserId,
        IReadOnlyCollection<Guid> EmployeeIds,
        int ActiveEmployees,
        bool CanViewAllProfiles,
        bool CanReviewProfiles,
        bool CanViewMinisterOffice,
        Guid? AllowedCountryId,
        (DateTime From, DateTime To) Range);

    private sealed record TeamAssignmentStat(
        Guid EmployeeId,
        int Assigned,
        int Active,
        int Completed,
        int Remaining,
        int Overdue);

    private sealed record TeamReviewStat(
        Guid EmployeeId,
        int Reviewed,
        int Approved,
        int Rejected);
}
