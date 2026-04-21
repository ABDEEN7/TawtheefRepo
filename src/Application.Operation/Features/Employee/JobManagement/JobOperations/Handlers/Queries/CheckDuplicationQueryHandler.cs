using Application.Operation.Common.Validations;
using Application.Operation.Features.Employee.JobManagement.JobOperations.Queries;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Handlers.Queries;

public class CheckDuplicationQueryHandler(IJobValidationService jobValidationService) : IRequestHandler<CheckDuplicationQuery, IResult<bool>>
{
    public async Task<IResult<bool>> Handle(CheckDuplicationQuery request, CancellationToken cancellationToken)
    {
        if(request.ManagementId == Guid.Empty || request.SectorId == Guid.Empty || request.JobTitleId == Guid.Empty || request.GenderId == Guid.Empty)
            return Result.Fail<bool>("Invalid input parameters.");
        
        var isDuplicated = await jobValidationService.IsDuplicateJob(null,
            request.ManagementId, request.SectorId, 
            request.JobTitleId, request.GenderId, request.DepartmentId);
        return Result.Ok(isDuplicated);
    }
}
