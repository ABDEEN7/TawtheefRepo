using Application.Operation.Features.Employee.Dashboard.DTOs.Jobs;
using Application.Operation.Features.Employee.Dashboard.Queries.Common;
using Application.Operation.Features.Employee.Dashboard.Services.Access;
using Application.Operation.Features.Employee.Dashboard.Services.Scopes;
using Application.Operation.Features.Employee.Dashboard.Services.Time;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Dashboard.Services.Read;

internal sealed class DashboardJobsReader(
    IUnitOfWork uow,
    ILocalizationService localizationService,
    DashboardAccessContextProvider accessContextProvider,
    DashboardQueryScope scope)
{
    public async Task<Result<IReadOnlyList<LatestJobDto>>> ReadLatestAsync(DashboardQueryBase request, CancellationToken ct)
    {
        var contextResult = await accessContextProvider.GetAsync(ct);
        if (contextResult.IsFailed) return Result.Fail(contextResult.Errors);
        var context = contextResult.Value;
        var range = DashboardTemporalResolver.ResolveRequestRange(
            request.Year, request.FromDateUtc, request.ToDateUtc, DateTime.UtcNow);
        var invitations = uow.GetEntityRepository<Invitation>().DbSet;
        var rows = await scope.Jobs(request, context, range)
            .OrderByDescending(job => job.UpdatedDate)
            .ThenByDescending(job => job.CreatedDate)
            .Take(4)
            .Select(job => new
            {
                job.Id,
                JobTitleAr = job.JobTitle != null ? job.JobTitle.JobNameAr : string.Empty,
                JobTitleEn = job.JobTitle != null ? job.JobTitle.JobNameEn : string.Empty,
                job.Management,
                Status = job.JobStatus != null ? job.JobStatus.BackendName : "N/A",
                CandidatesCount = invitations.Where(invitation => !invitation.IsDeleted && invitation.JobId == job.Id && invitation.IsAccepted)
                    .Select(invitation => invitation.ApplicantId).Distinct().Count(),
                InvitationsSent = context.CanViewInvitations
                    ? invitations.Count(invitation => !invitation.IsDeleted && invitation.JobId == job.Id)
                    : 0
            })
            .ToListAsync(ct);

        return Result.Ok<IReadOnlyList<LatestJobDto>>([
            .. rows.Select(job => new LatestJobDto
            {
                JobId = job.Id,
                JobTitle = localizationService.GetLocalizedValue(job.JobTitleAr, job.JobTitleEn),
                ManagementName = localizationService.GetLocalizedName(job.Management),
                Status = job.Status,
                CandidatesCount = job.CandidatesCount,
                InvitationsSent = job.InvitationsSent
            })
        ]);
    }
}
