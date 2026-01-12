using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Queries;


public record GetJobCountByJobStatsQuery(Guid JobStatusId) : IQuery<IResult<int>>;
