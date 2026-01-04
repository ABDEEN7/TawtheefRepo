using FluentValidation;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands.Validators;

public class CreateJobFromPreviousCommandValidator : AbstractValidator<CreateJobFromPreviousCommand>
{
    public CreateJobFromPreviousCommandValidator(IJobValidationService validationService)
    {
        RuleFor(x => x.Job)
            .NotNull()
            .WithMessage(JobMessages.JobRequired);

        RuleFor(x => x.SourceJobId)
            .NotEmpty()
            .WithMessage(JobMessages.JobRequired);

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
