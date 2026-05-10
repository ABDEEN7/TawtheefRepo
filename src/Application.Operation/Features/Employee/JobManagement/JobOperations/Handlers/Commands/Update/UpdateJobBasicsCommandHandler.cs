using Application.Operation.Common.Validations;
using Application.Operation.Features.Employee.JobManagement.JobOperations.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Handlers.Commands.Update;

public class UpdateJobBasicsCommandHandler(
    IJobRepository jobRepository,
    IJobValidationService validationService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateJobBasicsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateJobBasicsCommand request, CancellationToken cancellationToken)
    {
        // 1. Load the job
        var jobResult = await jobRepository.Repository.GetByIdAsync(request.JobId, cancellationToken);
        if (jobResult.IsFailed || jobResult.Value == null)
            return Result.Fail<Unit>(JobMessages.JobNotFound);

        var job = jobResult.Value;

        // 2. Validation
        var validationResult = await validationService.ValidateBasicsUpdate(request.Data, job);
        if (!validationResult.IsValid)
            return Result.Fail<Unit>(validationResult.Errors.First().ErrorMessage);

        // 4. Update basic properties
        job.JobTitleId = request.Data.JobTitleId;
        job.SectorId = request.Data.SectorId;
        job.ManagementId = request.Data.ManagementId;
        job.DepartmentId = request.Data.DepartmentId;
        job.YearsOfExperience = request.Data.YearsOfExperience;
        job.JobCategoryId = request.Data.JobCategoryId;
        job.WorkLocationId = request.Data.WorkLocationId;
        job.GenderId = request.Data.GenderId;
        job.WorkTypeId = request.Data.WorkTypeId;
        job.NumberOfVacancies = request.Data.NumberOfVacancies;
        job.ClosingDate = request.Data.ClosingDate.UtcDateTime;
        job.MinimumAge = request.Data.MinimumAge;
        job.MaximumAge = request.Data.MaximumAge;

        // 5. Persist
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail<Unit>(JobMessages.ConcurrencyConflict);
        }

        return Result.Ok(Unit.Value);
    }
}
