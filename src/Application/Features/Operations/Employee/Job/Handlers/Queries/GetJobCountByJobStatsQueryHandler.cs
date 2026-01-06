using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Queries;

public class GetJobCountByJobStatsQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetJobCountByJobStatsQuery, IResult<int>>
{
    public async Task<IResult<int>> Handle(
        GetJobCountByJobStatsQuery request,
        CancellationToken cancellationToken)
    {
        var jobRepo = unitOfWork.GetEntityRepository<JobEntity>();
        var count =  await jobRepo.DbSet
                .CountAsync( j => j.JobStatusId == request.JobStatusId, cancellationToken);

        return Result.Ok(count);

    }
}
