using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Lookups.Queries;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Queries;

public sealed record GetManagementsBySectorQuery(Guid SectorId)
    : BaseSearchQuery, IRequest<IResult<List<DropdownOptions>>>;

