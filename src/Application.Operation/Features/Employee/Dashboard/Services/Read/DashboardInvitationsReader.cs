using Application.Operation.Features.Employee.Dashboard.DTOs.Invitations;
using Application.Operation.Features.Employee.Dashboard.Queries.Common;
using Application.Operation.Features.Employee.Dashboard.Services.Access;
using Application.Operation.Features.Employee.Dashboard.Services.Scopes;
using Application.Operation.Features.Employee.Dashboard.Services.Time;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Services;

namespace Application.Operation.Features.Employee.Dashboard.Services.Read;

internal sealed class DashboardInvitationsReader(
    ILocalizationService localizationService,
    DashboardAccessContextProvider accessContextProvider,
    DashboardQueryScope scope)
{
    public async Task<Result<IReadOnlyList<LatestInvitationDto>>> ReadLatestAsync(DashboardQueryBase request, CancellationToken ct)
    {
        var contextResult = await accessContextProvider.GetAsync(ct);
        if (contextResult.IsFailed) return Result.Fail(contextResult.Errors);
        var context = contextResult.Value;
        if (!context.CanViewInvitations) return Result.Ok<IReadOnlyList<LatestInvitationDto>>([]);
        var range = DashboardTemporalResolver.ResolveRequestRange(
            request.Year, request.FromDateUtc, request.ToDateUtc, DateTime.UtcNow);

        var rows = await scope.Invitations(request, context, range)
            .OrderByDescending(invitation => invitation.CreatedDate)
            .Take(10)
            .Select(invitation => new
            {
                invitation.Id,
                invitation.CreatedDate,
                JobTitleAr = invitation.Job != null && invitation.Job.JobTitle != null ? invitation.Job.JobTitle.JobNameAr : string.Empty,
                JobTitleEn = invitation.Job != null && invitation.Job.JobTitle != null ? invitation.Job.JobTitle.JobNameEn : string.Empty,
                ApplicantNameAr = invitation.Applicant != null ? invitation.Applicant.FullNameAr : string.Empty,
                ApplicantNameEn = invitation.Applicant != null ? invitation.Applicant.FullNameEn : string.Empty,
                Status = invitation.InvitationStatus != null ? invitation.InvitationStatus.BackendName : "N/A"
            })
            .ToListAsync(ct);

        return Result.Ok<IReadOnlyList<LatestInvitationDto>>(rows.Select(invitation =>
        {
            var jobTitle = localizationService.GetLocalizedValue(invitation.JobTitleAr, invitation.JobTitleEn);
            var applicantName = localizationService.GetLocalizedValue(invitation.ApplicantNameAr, invitation.ApplicantNameEn);
            return new LatestInvitationDto
            {
                InvitationId = invitation.Id,
                Title = string.IsNullOrWhiteSpace(applicantName) ? jobTitle : $"{jobTitle} - {applicantName}",
                Status = invitation.Status,
                SentDate = invitation.CreatedDate,
                ActionKey = null
            };
        }).ToList());
    }
}
