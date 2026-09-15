using Application.Operation.Features.Employee.QuestionBankRequests.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.QuestionBankRequests.Validators;

public sealed class CreateQuestionBankRequestCommandValidator : AbstractValidator<CreateQuestionBankRequestCommand>
{
    public CreateQuestionBankRequestCommandValidator()
    {
        RuleFor(x => x.QuestionBankTypeId).NotEmpty();
        RuleFor(x => x.Reason).MaximumLength(2000);
    }
}
