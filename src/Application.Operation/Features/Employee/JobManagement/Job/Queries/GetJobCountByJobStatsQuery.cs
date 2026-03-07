using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Queries;


public record GetJobCountByJobStatsQuery(Guid JobStatusId) : IRequest<IResult<int>>;

