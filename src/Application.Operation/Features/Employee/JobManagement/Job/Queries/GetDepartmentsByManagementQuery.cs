using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Lookups.Queries;

namespace Application.Operation.Features.Employee.JobManagement.Job.Queries;

public sealed record GetDepartmentsByManagementQuery(Guid ManagementId)
    : BaseSearchQuery, IRequest<IResult<List<DropdownOptions>>>;

