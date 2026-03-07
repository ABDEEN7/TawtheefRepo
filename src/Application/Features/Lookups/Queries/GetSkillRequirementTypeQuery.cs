using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Lookups.Queries;

public sealed record GetSkillRequirementTypeQuery : BaseSearchQuery, IRequest<IResult<List<DropdownOptions>>>;


