using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Lookups.Queries;

public sealed record GetCitiesByCountryQuery(Guid CountryId) : BaseSearchQuery, IQuery<IResult<List<DropdownOptions>>>;
