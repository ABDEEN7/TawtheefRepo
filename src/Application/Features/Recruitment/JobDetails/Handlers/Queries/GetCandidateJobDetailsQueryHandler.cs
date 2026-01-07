using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;

using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Application.Features.Recruitment.JobDetails.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Recruitment.JobDetails.Handlers.Queries;

public sealed class GetCandidateJobDetailsQueryHandler(IUnitOfWork unitOfWork, IJobRepository jobRepository, IMapper mapper)
    : IQueryHandler<GetCandidateJobDetailsQuery, IResult<JobResponseDto>>
{
    public async Task<IResult<JobResponseDto>> Handle(GetCandidateJobDetailsQuery query, CancellationToken cancellationToken)
    {
        var invitation = await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(
                inv => inv.Id == query.InvitationId && inv.ApplicantId == query.UserId,
                cancellationToken);

        if (invitation is null)
            return Result.Fail<JobResponseDto>(ErrorsCodes.InvitationNotFound);

        var jobResult = await jobRepository.GetByIdWithDetailsAsync(invitation.JobId);

        if (jobResult.IsFailed)
            return Result.Fail<JobResponseDto>(jobResult.Errors);

        var job = jobResult.Value;

        if (job is null)
            return Result.Fail<JobResponseDto>(JobMessages.JobNotFound);

        var jobDto = mapper.Map<JobResponseDto>(job);
        return Result.Ok(jobDto);
    }
}
