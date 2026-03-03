using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Lookups.Queries;

namespace Application.Operation.Features.Employee.JobManagement.Job.Queries;

public record GetSkillBySubMajorIdAndRelatedParentSkillQuery(Guid SubMajorId) : BaseSearchQuery, IQuery<IResult<List<DropdownOptions>>>;
