using Application.Operation.Features.Employee.Interview.Evaluation.Queries;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.Evaluation.Validators;

public sealed class GetMemberEvaluationFormQueryValidator : AbstractValidator<GetMemberEvaluationFormQuery>
{
    public GetMemberEvaluationFormQueryValidator()
    {
        RuleFor(x => x.AppointmentId).NotEmpty();
    }
}
