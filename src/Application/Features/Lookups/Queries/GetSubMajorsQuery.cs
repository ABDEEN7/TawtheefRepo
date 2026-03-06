using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Lookups.Queries;

public sealed record GetSubMajorsQuery(Guid ParentId): BaseSearchQuery, IRequest<IResult<List<DropdownOptions>>>;

