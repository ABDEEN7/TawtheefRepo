using Application.Recruitment.Features.Dashboard.DTOs;
using Application.Recruitment.Features.Dashboard.Queries;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Recruitment.Features.Dashboard.Handlers.Queries;

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
                Received = g.Count(),
                Accepted = g.Count(i => i.InvitationStatusId == InvitationStatusIds.Submitted),
                Rejected = g.Count(i => i.InvitationStatusId == InvitationStatusIds.Rejected),
            })
            .FirstOrDefaultAsync(cancellationToken) ?? new CandidateInvitationStatisticsDto();

        return Result.Ok(statistics);
    }
}

