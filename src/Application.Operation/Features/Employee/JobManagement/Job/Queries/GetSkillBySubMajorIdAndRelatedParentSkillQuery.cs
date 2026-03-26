using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Lookups.Queries;

namespace Application.Operation.Features.Employee.JobManagement.Job.Queries;

public record GetSkillBySubMajorIdAndRelatedParentSkillQuery(Guid? SubMajorId) : BaseSearchQuery, IRequest<IResult<List<DropdownOptions>>>;

