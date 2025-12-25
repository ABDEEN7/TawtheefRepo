using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Queries;

public class GetJobCountByJobStatsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetJobCountByJobStatsQuery, int>
{
    public async Task<int> Handle(
        GetJobCountByJobStatsQuery request,
        CancellationToken cancellationToken)
    {
        var jobRepo = unitOfWork.GetEntityRepository<JobEntity>();
        var count =  await jobRepo.DbSet
                .AsNoTracking()
                .CountAsync( j => j.JobStatusId == request.JobStatusId, cancellationToken);

        return count;

    }
}
