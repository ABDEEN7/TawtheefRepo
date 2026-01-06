using Cortex.Mediator.Queries;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Queries;


public record GetJobCountByJobStatsQuery(Guid JobStatusId) : IQuery<IResult<int>>;
