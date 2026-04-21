using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Queries;


public record GetJobCountByJobStatsQuery(Guid JobStatusId) : IRequest<IResult<int>>;

