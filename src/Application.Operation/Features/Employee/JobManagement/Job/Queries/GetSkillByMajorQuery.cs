using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Lookups.Queries;

namespace Application.Operation.Features.Employee.JobManagement.Job.Queries;

public record GetSkillByMajorQuery(Guid MajorId) : BaseSearchQuery, IQuery<IResult<List<DropdownOptions>>>;
