using Application.Operation.Features.Employee.JobManagement.JobOperations.Queries;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Handlers.Queries;

public class GetJobCountByJobStatsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetJobCountByJobStatsQuery, IResult<int>>
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

