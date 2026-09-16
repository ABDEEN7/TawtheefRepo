using Application.Operation.Features.Employee.Interview.Committee.Queries;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.Committee.Validators;

public sealed class ListCommitteeMembersQueryValidator : AbstractValidator<ListCommitteeMembersQuery>
{
    public ListCommitteeMembersQueryValidator()
    {
        RuleFor(x => x.InterviewCommitteeId).NotEmpty();
    }
}
