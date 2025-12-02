using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Lookups.Queries;

public sealed record GetDepartmentsQuery: BaseSearchQuery, IRequest<IResult<List<DropdownOptions>>>;
