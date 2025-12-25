using FluentResults;
using MediatR;


namespace Tawtheef.Application.Features.Operations.Employee.Job.Queries;


public record GetJobCountByJobStatsQuery(Guid JobStatusId) : IRequest<IResult<int>>;
