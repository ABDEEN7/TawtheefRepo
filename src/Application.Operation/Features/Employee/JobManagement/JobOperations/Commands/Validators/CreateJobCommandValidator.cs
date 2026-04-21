using Application.Operation.Common.Validations;
using FluentValidation;
using Tawtheef.Domain.Constants;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Commands.Validators;

public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
{

    public CreateJobCommandValidator(IJobValidationService validationService)
    {
        RuleFor(x => x.Job)
            .NotNull()
            .WithMessage("Job is required")
            .WithErrorCode(JobMessages.JobRequired);

        RuleFor(x => x)
            .CustomAsync(async (command, context, _) =>
            {
                var validationResult = await validationService.ValidateForCreation(command.Job);

                if (!validationResult.IsValid)
                {
                    foreach (var error in validationResult.Errors)
                    {
                        context.AddFailure(new FluentValidation.Results.ValidationFailure
                        {
                            PropertyName = error.PropertyName ?? "Job",
                            ErrorMessage = error.ErrorMessage,
                            ErrorCode = string.IsNullOrWhiteSpace(error.ErrorCode)
                                ? ErrorsCodes.ValidationError
                                : error.ErrorCode
                        });
                    }
                }
            });
    }
}
