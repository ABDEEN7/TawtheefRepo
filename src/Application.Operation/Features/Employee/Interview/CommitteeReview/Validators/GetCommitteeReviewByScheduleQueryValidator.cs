using Application.Operation.Features.Employee.Interview.CommitteeReview.Queries;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.CommitteeReview.Validators;

public sealed class GetCommitteeReviewByScheduleQueryValidator : AbstractValidator<GetCommitteeReviewByScheduleQuery>
{
    public GetCommitteeReviewByScheduleQueryValidator()
    {
        RuleFor(x => x.ScheduleId).NotEmpty();
    }
}
