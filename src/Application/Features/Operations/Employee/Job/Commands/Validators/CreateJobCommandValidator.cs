using FluentValidation;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands.Validators;

public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
{

    public CreateJobCommandValidator(IJobValidationService validationService)
    {
        RuleFor(x => x.Job)
            .NotNull()
            .WithMessage(JobMessages.JOB_REQUIRED);

        RuleFor(x => x)
            .CustomAsync(async (command, context, _) =>
            {
                var validationResult = await validationService.ValidateForCreation(command.Job);

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
