using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using FluentValidation;

namespace Application.Operation.Features.Employee.Interview.Schedule.Validators;

public sealed class SlotAssignmentDtoValidator : AbstractValidator<SlotAssignmentDto>
{
    public SlotAssignmentDtoValidator()
    {
        RuleFor(x => x.InvitationId).NotEmpty();
    }
}
