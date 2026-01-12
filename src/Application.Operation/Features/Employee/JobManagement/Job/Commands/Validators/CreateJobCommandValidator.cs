using Application.Operation.Common.Validations;
using FluentValidation;
using Tawtheef.Domain.Constants;

namespace Application.Operation.Features.Employee.JobManagement.Job.Commands.Validators;

public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
{

    public CreateJobCommandValidator(IJobValidationService validationService)
    {
        RuleFor(x => x.Job)
            .NotNull()
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
