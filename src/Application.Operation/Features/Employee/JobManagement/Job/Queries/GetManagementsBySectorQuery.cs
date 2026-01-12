using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Lookups.Queries;

namespace Application.Operation.Features.Employee.JobManagement.Job.Queries;

public sealed record GetManagementsBySectorQuery(Guid SectorId)
    : BaseSearchQuery, IQuery<IResult<List<DropdownOptions>>>;
