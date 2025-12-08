using FluentValidation;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands.Validators;

public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
{
    private readonly IJobValidationService _validationService;

    public CreateJobCommandValidator(IJobValidationService validationService)
    {
        _validationService = validationService;

        RuleFor(x => x.Job)
            .NotNull()
            .WithMessage(JobValidationMessages.JOB_REQUIRED);

        RuleFor(x => x)
            .CustomAsync(async (command, context, cancellationToken) =>
            {
                var validationResult = await _validationService.ValidateForCreation(command.Job);

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
