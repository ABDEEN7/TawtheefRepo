using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Lookups.Queries;

public sealed record GetOfficesQuery(Guid? CountryId = null) : BaseSearchQuery, IQuery<IResult<List<DropdownOptions>>>;
