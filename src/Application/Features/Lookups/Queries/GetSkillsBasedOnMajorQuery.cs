using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Lookups.Queries;

public sealed record GetSkillsBasedOnMajorQuery(List<Guid> Majors) : BaseSearchQuery, IQuery<IResult<List<DropdownOptions>>>;
