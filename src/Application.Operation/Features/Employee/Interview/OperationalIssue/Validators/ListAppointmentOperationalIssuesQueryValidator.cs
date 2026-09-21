using Application.Operation.Features.Employee.Interview.OperationalIssue.Queries;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.OperationalIssue.Validators;

public sealed class ListAppointmentOperationalIssuesQueryValidator : AbstractValidator<ListAppointmentOperationalIssuesQuery>
{
    public ListAppointmentOperationalIssuesQueryValidator()
    {
        RuleFor(x => x.AppointmentId).NotEmpty();
    }
}
