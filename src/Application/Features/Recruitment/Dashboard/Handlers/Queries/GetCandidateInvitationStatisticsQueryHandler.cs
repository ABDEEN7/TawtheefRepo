using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Dashboard.DTOs;
using Tawtheef.Application.Features.Recruitment.Dashboard.Queries;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Recruitment.Dashboard.Handlers.Queries;

public sealed class GetCandidateInvitationStatisticsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetCandidateInvitationStatisticsQuery, IResult<CandidateInvitationStatisticsDto>>
{
    public async Task<IResult<CandidateInvitationStatisticsDto>> Handle(
        GetCandidateInvitationStatisticsQuery query,
        CancellationToken cancellationToken)
    {
        var statistics = await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .Where(invitation => invitation.ApplicantId == query.UserId)
            .GroupBy(_ => 1)
            .Select(g => new CandidateInvitationStatisticsDto
            {
                NewInvitations = g.Count(i => i.InvitationStatusId == InvitationStatusIds.NewInvitation),
                UnderReview = g.Count(i => i.InvitationStatusId == InvitationStatusIds.UnderReview),
                Withdrawn = g.Count(i => i.InvitationStatusId == InvitationStatusIds.Cancelled),
                Applied = g.Count(i => i.InvitationStatusId == InvitationStatusIds.Submitted)
            })
            .FirstOrDefaultAsync(cancellationToken) ?? new CandidateInvitationStatisticsDto();

        return Result.Ok(statistics);
    }
}
