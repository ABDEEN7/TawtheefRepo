using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Queries;

public record CheckDuplicationQuery(Guid ManagementId, Guid SectorId, Guid JobTitleId, 
    Guid GenderId, Guid? DepartmentId = null) : IRequest<IResult<bool>>;
