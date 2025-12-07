using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Lookups.Queries;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Queries;

public record GetSkillByMajorQuery(Guid majorId) : BaseSearchQuery, IRequest<IResult<List<DropdownOptions>>>;
