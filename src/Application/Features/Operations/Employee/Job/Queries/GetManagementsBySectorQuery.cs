using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Lookups.Queries;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Queries;

public sealed record GetManagementsBySectorQuery(Guid SectorId)
    : BaseSearchQuery, IRequest<IResult<List<DropdownOptions>>>;
