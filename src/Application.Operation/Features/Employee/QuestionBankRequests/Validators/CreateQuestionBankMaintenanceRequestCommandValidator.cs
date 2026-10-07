using Application.Operation.Features.Employee.QuestionBankRequests.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.QuestionBankRequests.Validators;

public sealed class CreateQuestionBankMaintenanceRequestCommandValidator
    : AbstractValidator<CreateQuestionBankMaintenanceRequestCommand>
{
    public CreateQuestionBankMaintenanceRequestCommandValidator()
    {
        RuleFor(x => x.QuestionBankId).NotEmpty();
        RuleFor(x => x.Reason).MaximumLength(2000);
    }
}
