using Application.Operation.Features.Employee.Interview.OperationalIssue.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.OperationalIssue.Validators;

public sealed class CreateOperationalIssueCommandValidator : AbstractValidator<CreateOperationalIssueCommand>
{
    public CreateOperationalIssueCommandValidator()
    {
        RuleFor(x => x.AppointmentId).NotEmpty();
        RuleFor(x => x.IssueType).IsInEnum();
        RuleFor(x => x.Description).MaximumLength(2000);
    }
}
