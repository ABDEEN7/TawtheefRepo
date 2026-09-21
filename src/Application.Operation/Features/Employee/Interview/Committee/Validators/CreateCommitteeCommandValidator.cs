using Application.Operation.Features.Employee.Interview.Committee.Commands;
using Application.Operation.Features.Employee.Interview.Committee.Validators;
using FluentValidation;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Committee.Validators;

public sealed class CreateCommitteeCommandValidator : AbstractValidator<CreateCommitteeCommand>
{
    public CreateCommitteeCommandValidator()
    {
        RuleFor(x => x.JobId).NotEmpty();
        RuleFor(x => x.InterviewTemplateId).NotEmpty();
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NameEn).MaximumLength(200);
        RuleFor(x => x.ScopeDescription).MaximumLength(1000);
        RuleFor(x => x.Notes).MaximumLength(1000);

        RuleForEach(x => x.Members).SetValidator(new MemberInputDtoValidator());
        RuleFor(x => x.Members)
            .Must(members => members.Select(m => m.MemberUserId).Distinct().Count() == members.Count)
            .WithMessage("A user cannot appear more than once in the member list.");
        RuleFor(x => x.Members)
            .Must(members => members.Count(m => m.Role == CommitteeRole.Chair) <= 1)
            .WithMessage("Only one member may be the Chair.");
    }
}
