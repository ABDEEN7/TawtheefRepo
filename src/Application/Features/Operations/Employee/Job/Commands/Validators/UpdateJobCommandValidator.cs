using FluentValidation;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands.Validators;

public class UpdateJobCommandValidator : AbstractValidator<UpdateJobCommand>
{
    public UpdateJobCommandValidator(IJobRepository jobRepository, IJobValidationService validationService)
    {
        RuleFor(x => x.Job)
                    .NotNull()
                    .WithMessage(JobValidationMessages.JOB_REQUIRED);

        RuleFor(x => x)
            .CustomAsync(async (command, context, _) =>
            {
                var job = await jobRepository.GetByIdWithDetailsAsync(command.JobId);
                var validationResult = await validationService.ValidateForUpdate(command.Job, job.Value);

                if (!validationResult.IsValid)
                {
                    foreach (var error in validationResult.Errors)
                    {
                        context.AddFailure(error);
                    }
                }
            });
    }
}
