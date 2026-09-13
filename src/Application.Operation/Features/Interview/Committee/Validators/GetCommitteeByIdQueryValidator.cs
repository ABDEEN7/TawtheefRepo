using Application.Operation.Features.Interview.Committee.Queries;
using FluentValidation;

namespace Application.Operation.Features.Interview.Committee.Validators;

public sealed class GetCommitteeByIdQueryValidator : AbstractValidator<GetCommitteeByIdQuery>
{
    public GetCommitteeByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
