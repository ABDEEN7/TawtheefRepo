using Application.Operation.Common.Validations;
using FluentValidation;
using Tawtheef.Domain.Constants;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Commands.Validators;

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
