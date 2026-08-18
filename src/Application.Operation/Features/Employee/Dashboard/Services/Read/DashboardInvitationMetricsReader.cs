using Application.Operation.Features.Employee.Dashboard.DTOs.Invitations;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Dashboard.Services.Read;

internal sealed class DashboardInvitationMetricsReader
{
    public async Task<DashboardInvitationMetrics> ReadAsync(
        IQueryable<Job> jobs,
        DateTime currentFrom,
        bool canViewInvitations,
        CancellationToken ct)
    {
        if (!canViewInvitations) jobs = jobs.Where(_ => false);
        var rows = await jobs
            .SelectMany(job => job.Invitations
                .Where(invitation => !invitation.IsDeleted)
                .Select(invitation => new
                {
                    IsCurrent = job.CreatedDate >= currentFrom, invitation.InvitationStatusId, invitation.IsAccepted
                }))
            .GroupBy(row => new { row.IsCurrent, row.InvitationStatusId, row.IsAccepted })
            .Select(group => new
            {
                group.Key.IsCurrent, group.Key.InvitationStatusId, group.Key.IsAccepted, Count = group.Count()
            })
            .ToListAsync(ct);

        InvitationKpisDto Build(bool isCurrent)
        {
            var periodRows = rows.Where(row => row.IsCurrent == isCurrent).ToList();
            var expired = periodRows.Where(row => !row.IsAccepted &&
                                                  (row.InvitationStatusId == InvitationStatusIds.Closed ||
                                                   row.InvitationStatusId == InvitationStatusIds.Cancelled ||
                                                   row.InvitationStatusId == InvitationStatusIds.Expired))
                .Sum(row => row.Count);
            return new InvitationKpisDto
            {
                TotalInvitations = periodRows.Sum(row => row.Count),
                AcceptedInvitations = periodRows.Where(row => row.IsAccepted).Sum(row => row.Count),
                PendingInvitations = periodRows.Where(row => !row.IsAccepted &&
                                                             row.InvitationStatusId != InvitationStatusIds.Rejected &&
                                                             row.InvitationStatusId != InvitationStatusIds.Closed &&
                                                             row.InvitationStatusId != InvitationStatusIds.Cancelled &&
                                                             row.InvitationStatusId != InvitationStatusIds.Expired &&
                                                             row.InvitationStatusId != InvitationStatusIds
                                                                 .PendingAttachmentApproval)
                    .Sum(row => row.Count),
                PendingAttachmentApproval = periodRows.Where(row =>
                    row.InvitationStatusId == InvitationStatusIds.PendingAttachmentApproval).Sum(row => row.Count),
                ExpiredInvitations = expired,
                RejectedInvitations = periodRows.Where(row => !row.IsAccepted &&
                                                              row.InvitationStatusId == InvitationStatusIds.Rejected)
                    .Sum(row => row.Count)
            };
        }

        return new DashboardInvitationMetrics(
            new PeriodValues<InvitationKpisDto>(Build(true), Build(false)));
    }
}
