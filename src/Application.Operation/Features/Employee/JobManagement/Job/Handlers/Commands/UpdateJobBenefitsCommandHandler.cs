using Application.Operation.Features.Employee.JobManagement.Job.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Domain.Constants;
using Application.Operation.Common.Validations;

namespace Application.Operation.Features.Employee.JobManagement.Job.Handlers.Commands;

public class UpdateJobBenefitsCommandHandler(
    IJobRepository jobRepository,
    IJobValidationService validationService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateJobBenefitsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateJobBenefitsCommand request, CancellationToken cancellationToken)
    {
        // 1. Load the job (scalar-only update, no collections needed)
        var jobResult = await jobRepository.Repository.GetByIdAsync(request.JobId, cancellationToken);
        if (jobResult.IsFailed || jobResult.Value == null)
            return Result.Fail<Unit>(JobMessages.JobNotFound);

        var job = jobResult.Value;

        var validationResult = await validationService.ValidateBenefitsUpdate(request.Data, job);
        if (!validationResult.IsValid)
            return Result.Fail<Unit>(validationResult.Errors.First().ErrorMessage);

        // 4. Update only benefits fields
        job.BenefitsAr = request.Data.BenefitsAr;
        job.BenefitsEn = request.Data.BenefitsEn;

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
