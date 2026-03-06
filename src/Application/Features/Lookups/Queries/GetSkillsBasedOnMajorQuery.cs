using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Lookups.Queries;

public sealed record GetSkillsBasedOnMajorQuery(List<Guid> Majors) : BaseSearchQuery, IRequest<IResult<List<DropdownOptions>>>;

