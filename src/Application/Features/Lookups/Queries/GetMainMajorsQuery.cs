using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Lookups.Queries;

public sealed record GetMainMajorsQuery(bool IncludeOrphanMajors): BaseSearchQuery, IRequest<IResult<List<DropdownOptions>>>;

