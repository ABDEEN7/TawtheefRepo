using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Employee.Job.Queries;


public record GetJobCountByJobStatsQuery(Guid JobStatusId) : IQuery<IResult<int>>;
