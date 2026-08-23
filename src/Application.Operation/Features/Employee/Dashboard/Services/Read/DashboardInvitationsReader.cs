using Application.Operation.Features.Employee.Dashboard.DTOs.Invitations;
using Application.Operation.Features.Employee.Dashboard.Queries.Common;
using Application.Operation.Features.Employee.Dashboard.Queries.Invitations;
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

internal sealed class DashboardInvitationsReader(
    IUnitOfWork uow,
    ILocalizationService localizationService,
    DashboardAccessContextProvider accessContextProvider,
    DashboardQueryScope scope)
{
    public async Task<Result<PaginatedResult<LatestInvitationDto>>> ReadLatestAsync(
        GetLatestInvitationsQuery request,
        CancellationToken ct)
    {
        var contextResult = await accessContextProvider.GetAsync(ct);
        if (contextResult.IsFailed)
            return Result.Fail(contextResult.Errors);

        var context = contextResult.Value;

        if (!context.CanViewInvitations)
        {
            return Result.Ok(
                new PaginatedResult<LatestInvitationDto>(
                    [],
                    0,
                    request.PageNumber,
                    request.PageSize));
        }

        var range = DashboardTemporalResolver.ResolveRequestRange(
            request.Year,
            request.FromDateUtc,
            request.ToDateUtc,
            DateTime.UtcNow);

        var query = BuildQuery(
            request,
            context,
            range.FromUtc,
            range.ToExclusiveUtc);

        var total = await query.CountAsync(ct);

        var pagedQuery = query
            .OrderByDescending(invitation => invitation.CreatedDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        var items = await ReadInvitationsAsync(
            pagedQuery,
            ct);

        return Result.Ok(
            new PaginatedResult<LatestInvitationDto>(
                items,
                total,
                request.PageNumber,
                request.PageSize));
    }

    public async Task<Result<IReadOnlyList<LatestInvitationDto>>> ReadExportAsync(
        DashboardQueryBase request,
        CancellationToken ct)
    {
        var contextResult = await accessContextProvider.GetAsync(ct);
        if (contextResult.IsFailed)
            return Result.Fail(contextResult.Errors);

        var context = contextResult.Value;

        if (!context.CanViewInvitations)
            return Result.Ok<IReadOnlyList<LatestInvitationDto>>([]);

        var range = DashboardTemporalResolver.ResolveRequestRange(
            request.Year,
            request.FromDateUtc,
            request.ToDateUtc,
            DateTime.UtcNow);

        var query = BuildQuery(
                request,
                context,
                range.FromUtc,
                range.ToExclusiveUtc)
            .OrderByDescending(invitation => invitation.CreatedDate);

        var items = await ReadInvitationsAsync(
            query,
            ct);

        return Result.Ok<IReadOnlyList<LatestInvitationDto>>(items);
    }

    private IQueryable<Invitation> BuildQuery(
        DashboardQueryBase request,
        DashboardAccessContext context,
        DateTime fromUtc,
        DateTime toExclusiveUtc)
    {
        IQueryable<Job> allowedJobs = scope.AccessibleJobs(context);

        if (request.DepartmentId.HasValue)
        {
            var departmentId = request.DepartmentId.Value;

            allowedJobs = allowedJobs.Where(job =>
                job.DepartmentId == departmentId);
        }

        var allowedJobIds = allowedJobs
            .Select(job => job.Id);

        return uow.GetEntityRepository<Invitation>()
            .DbSet
            .AsNoTracking()
            .Where(invitation =>
                !invitation.IsDeleted &&
                invitation.CreatedDate >= fromUtc &&
                invitation.CreatedDate < toExclusiveUtc &&
                allowedJobIds.Contains(invitation.JobId));
    }

    private async Task<List<LatestInvitationDto>> ReadInvitationsAsync(
        IQueryable<Invitation> query,
        CancellationToken ct)
    {
        var rows = await query
            .Select(invitation => new
            {
                invitation.Id,
                invitation.Source,
                invitation.CreatedDate,

                JobTitleAr =
                    invitation.Job != null &&
                    invitation.Job.JobTitle != null
                        ? invitation.Job.JobTitle.JobNameAr
                        : string.Empty,

                JobTitleEn =
                    invitation.Job != null &&
                    invitation.Job.JobTitle != null
                        ? invitation.Job.JobTitle.JobNameEn
                        : string.Empty,

                ApplicantNameAr =
                    invitation.Applicant != null
                        ? invitation.Applicant.FullNameAr
                        : string.Empty,

                ApplicantNameEn =
                    invitation.Applicant != null
                        ? invitation.Applicant.FullNameEn
                        : string.Empty,

                Status =
                    invitation.InvitationStatus != null
                        ? invitation.InvitationStatus.BackendName
                        : "N/A"
            })
            .ToListAsync(ct);

        return
        [
            .. rows
                .Select(invitation =>
                {
                    var jobTitle = localizationService.GetLocalizedValue(
                        invitation.JobTitleAr,
                        invitation.JobTitleEn);

                    var applicantName = localizationService.GetLocalizedValue(
                        invitation.ApplicantNameAr,
                        invitation.ApplicantNameEn);

                    return new LatestInvitationDto
                    {
                        InvitationId = invitation.Id,
                        Source = invitation.Source,
                        Title = string.IsNullOrWhiteSpace(applicantName)
                            ? jobTitle
                            : $"{jobTitle} - {applicantName}",
                        Status = invitation.Status,
                        SentDate = invitation.CreatedDate,
                        ActionKey = null
                    };
                })
        ];
    }
}
