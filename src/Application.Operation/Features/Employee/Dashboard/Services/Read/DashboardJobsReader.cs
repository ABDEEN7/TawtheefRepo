using Application.Operation.Features.Employee.Dashboard.DTOs.Jobs;
using Application.Operation.Features.Employee.Dashboard.Queries.Common;
using Application.Operation.Features.Employee.Dashboard.Queries.Jobs;
using Application.Operation.Features.Employee.Dashboard.Services.Access;
using Application.Operation.Features.Employee.Dashboard.Services.Scopes;
using Application.Operation.Features.Employee.Dashboard.Services.Time;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Dashboard.Services.Read;

internal sealed class DashboardJobsReader(
    IUnitOfWork uow,
    ILocalizationService localizationService,
    DashboardAccessContextProvider accessContextProvider,
    DashboardQueryScope scope)
{
    public async Task<Result<PaginatedResult<LatestJobDto>>> ReadLatestAsync(
        GetLatestJobsQuery request,
        CancellationToken ct)
    {
        var contextResult = await accessContextProvider.GetAsync(ct);
        if (contextResult.IsFailed)
            return Result.Fail(contextResult.Errors);

        var context = contextResult.Value;

        var range = DashboardTemporalResolver.ResolveRequestRange(
            request.Year,
            request.FromDateUtc,
            request.ToDateUtc,
            DateTime.UtcNow);

        var query = scope.Jobs(request, context, range);

        var total = await query.CountAsync(ct);

        var pagedQuery = query
            .OrderByDescending(job => job.UpdatedDate)
            .ThenByDescending(job => job.CreatedDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        var items = await ReadJobsAsync(
            pagedQuery,
            context,
            ct);

        return Result.Ok(
            new PaginatedResult<LatestJobDto>(
                items,
                total,
                request.PageNumber,
                request.PageSize));
    }
    
    public async Task<Result<IReadOnlyList<LatestJobDto>>> ReadExportAsync(
        DashboardQueryBase request,
        CancellationToken ct)
    {
        var contextResult = await accessContextProvider.GetAsync(ct);
        if (contextResult.IsFailed)
            return Result.Fail(contextResult.Errors);

        var context = contextResult.Value;

        var range = DashboardTemporalResolver.ResolveRequestRange(
            request.Year,
            request.FromDateUtc,
            request.ToDateUtc,
            DateTime.UtcNow);

        var query = scope.Jobs(request, context, range)
            .OrderByDescending(job => job.UpdatedDate)
            .ThenByDescending(job => job.CreatedDate);

        var jobs = await ReadJobsAsync(query, context, ct);

        return Result.Ok<IReadOnlyList<LatestJobDto>>(jobs);
    }
    
    private async Task<List<LatestJobDto>> ReadJobsAsync(
    IQueryable<Job> jobsQuery,
    DashboardAccessContext context,
    CancellationToken ct)
{
    var invitations = uow.GetEntityRepository<Invitation>()
        .DbSet
        .AsNoTracking();

    var jobIdsQuery = jobsQuery
        .Select(job => job.Id);

    var rows = await jobsQuery
        .Select(job => new
        {
            job.Id,

            JobTitleAr = job.JobTitle != null
                ? job.JobTitle.JobNameAr
                : string.Empty,

            JobTitleEn = job.JobTitle != null
                ? job.JobTitle.JobNameEn
                : string.Empty,

            job.Management,

            Status = job.JobStatus != null
                ? job.JobStatus.BackendName
                : "N/A",

            CandidatesCount = context.CanViewInvitations
                ? invitations
                    .Where(invitation =>
                        !invitation.IsDeleted &&
                        invitation.JobId == job.Id &&
                        invitation.IsAccepted)
                    .Select(invitation => invitation.ApplicantId)
                    .Distinct()
                    .Count()
                : 0,

            InvitationsSent = context.CanViewInvitations
                ? invitations.Count(invitation =>
                    !invitation.IsDeleted &&
                    invitation.JobId == job.Id)
                : 0
        })
        .ToListAsync(ct);

    var invitationWorkflow = context.CanViewInvitations
        ? await invitations
            .Where(invitation =>
                !invitation.IsDeleted &&
                jobIdsQuery.Contains(invitation.JobId))
            .GroupBy(invitation => new
            {
                invitation.JobId,
                Status = invitation.InvitationStatus != null
                    ? invitation.InvitationStatus.BackendName
                    : "N/A"
            })
            .Select(group => new
            {
                group.Key.JobId,
                group.Key.Status,
                Count = group.Count()
            })
            .ToListAsync(ct)
        : [];

    return
    [
        .. rows
            .Select(job => new LatestJobDto
            {
                JobId = job.Id,
                JobTitle = localizationService.GetLocalizedValue(
                    job.JobTitleAr,
                    job.JobTitleEn),
                ManagementName = localizationService.GetLocalizedName(
                    job.Management),
                Status = job.Status,
                CandidatesCount = job.CandidatesCount,
                InvitationsSent = job.InvitationsSent,
                InvitationWorkflow =
                [
                    .. invitationWorkflow
                        .Where(item => item.JobId == job.Id)
                        .Select(item => new InvitationStatusCountDto { Status = item.Status, Count = item.Count })
                ]
            })
    ];
}
    
}
