using Application.Operation.Features.Employee.Interview.Committee.Queries;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.Committee.Validators;

public sealed class ListEligibleCommitteeMembersQueryValidator : AbstractValidator<ListEligibleCommitteeMembersQuery>
{
    public ListEligibleCommitteeMembersQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(100);
    }
}
