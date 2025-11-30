using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Lookups.Queries;

public sealed record GetJobStatusesQuery: BaseSearchQuery, IRequest<IResult<List<DropdownOptions>>>;
