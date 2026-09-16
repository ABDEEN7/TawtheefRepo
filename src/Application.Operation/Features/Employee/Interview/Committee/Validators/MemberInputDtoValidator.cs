using Application.Operation.Features.Employee.Interview.Committee.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.Committee.Validators;

public sealed class MemberInputDtoValidator : AbstractValidator<MemberInputDto>
{
    public MemberInputDtoValidator()
    {
        RuleFor(x => x.MemberUserId).NotEmpty();
        RuleFor(x => x.Role).IsInEnum();
    }
}
