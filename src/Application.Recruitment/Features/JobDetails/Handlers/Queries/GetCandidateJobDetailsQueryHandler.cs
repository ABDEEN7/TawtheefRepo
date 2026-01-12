using Application.Recruitment.Features.JobDetails.DTOs;
using Application.Recruitment.Features.JobDetails.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Recruitment.Features.JobDetails.Handlers.Queries;

public sealed class GetCandidateJobDetailsQueryHandler(
    IUnitOfWork unitOfWork,
    IJobRepository jobRepository,
    IMapper mapper)
    : IQueryHandler<GetCandidateJobDetailsQuery, IResult<CandidateJobDetailsDto>>
{
    public async Task<IResult<CandidateJobDetailsDto>> Handle(GetCandidateJobDetailsQuery query, CancellationToken cancellationToken)
    {
        var invitation = await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(inv => inv.Id == query.InvitationId && inv.ApplicantId == query.UserId, cancellationToken);

        if (invitation is null)
            return Result.Fail<CandidateJobDetailsDto>(ErrorsCodes.InvitationNotFound);

        var jobResult = await jobRepository.GetByIdWithDetailsAsync(invitation.JobId);

        if (jobResult.IsFailed)
            return Result.Fail<CandidateJobDetailsDto>(jobResult.Errors);

        var job = jobResult.Value;

        if (job is null)
            return Result.Fail<CandidateJobDetailsDto>(JobMessages.JobNotFound);

        var jobDto = mapper.Map<CandidateJobDetailsDto>(job);

        return Result.Ok(jobDto);
    }
}
