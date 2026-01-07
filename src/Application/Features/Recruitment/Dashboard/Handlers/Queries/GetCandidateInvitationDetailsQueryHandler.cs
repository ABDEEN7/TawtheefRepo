using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;

using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Dashboard.DTOs;
using Tawtheef.Application.Features.Recruitment.Dashboard.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Recruitment.Dashboard.Handlers.Queries;

public sealed class GetCandidateInvitationDetailsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IQueryHandler<GetCandidateInvitationDetailsQuery, IResult<CandidateInvitationsDto>>
{
    public async Task<IResult<CandidateInvitationsDto>> Handle(
        GetCandidateInvitationDetailsQuery query,
        CancellationToken cancellationToken)
    {
        var invitation = await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .Include(i => i.InvitationStatus)
            .Include(i => i.Job).ThenInclude(j => j!.JobCategory)
            .Include(i => i.Job).ThenInclude(j => j!.Department)
            .FirstOrDefaultAsync(
                i => i.Id == query.InvitationId && i.ApplicantId == query.UserId,
                cancellationToken);

        if (invitation is null)
            return Result.Fail<CandidateInvitationsDto>(ErrorsCodes.InvitationNotFound);

        var dto = mapper.Map<CandidateInvitationsDto>(invitation);
        return Result.Ok(dto);
    }
}
