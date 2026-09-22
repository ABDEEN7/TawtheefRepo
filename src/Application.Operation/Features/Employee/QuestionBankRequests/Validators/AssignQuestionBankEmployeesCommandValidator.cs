using Application.Operation.Features.Employee.QuestionBankRequests.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.QuestionBankRequests.Validators;

public sealed class AssignQuestionBankEmployeesCommandValidator : AbstractValidator<AssignQuestionBankEmployeesCommand>
{
    public AssignQuestionBankEmployeesCommandValidator()
    {
        RuleFor(x => x.RequestId).NotEmpty();
        RuleFor(x => x.Assignments).NotEmpty();
        RuleForEach(x => x.Assignments).ChildRules(item =>
        {
            item.RuleFor(x => x.EmployeeId).NotEmpty();
            item.RuleFor(x => x.MinimumQuestionCount).GreaterThan(0);
            item.RuleFor(x => x.Notes).MaximumLength(1000);
        });
        RuleFor(x => x.Assignments).Must(x => x.Select(y => y.EmployeeId).Distinct().Count() == x.Count)
            .WithMessage("DUPLICATE_QUESTION_BANK_ASSIGNEE");
    }
}
