using Application.Operation.Features.Employee.Interview.Evaluation.Queries;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.Evaluation.Validators;

public sealed class ListAppointmentEvaluationsQueryValidator : AbstractValidator<ListAppointmentEvaluationsQuery>
{
    public ListAppointmentEvaluationsQueryValidator()
    {
        RuleFor(x => x.AppointmentId).NotEmpty();
    }
}
