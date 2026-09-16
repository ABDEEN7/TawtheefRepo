using Application.Operation.Features.Employee.TestSlots.Commands;
using FluentValidation;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.TestSlots.Validators;

public sealed class CreateTestSlotCommandValidator : AbstractValidator<CreateTestSlotCommand>
{
    public CreateTestSlotCommandValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.TestSlot).NotNull();
        When(x => x.TestSlot != null, () =>
        {
            RuleFor(x => x.TestSlot.TitleAr).NotEmpty().MaximumLength(200);
            RuleFor(x => x.TestSlot.TitleEn).MaximumLength(200);
            RuleFor(x => x.TestSlot.RoomId).NotEmpty();
            RuleFor(x => x.TestSlot.SlotDate)
                .NotEqual(default(DateOnly))
                .GreaterThanOrEqualTo(_ => DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime));
            RuleFor(x => x.TestSlot.StartTime).NotEqual(default(TimeOnly));
            RuleFor(x => x.TestSlot.EndTime).GreaterThan(x => x.TestSlot.StartTime);
            RuleFor(x => x.TestSlot.Staff).NotEmpty();
            RuleFor(x => x).Must(HasValidStaff).WithMessage(ErrorsCodes.InvalidRequest);
        });
    }

    private static bool HasValidStaff(CreateTestSlotCommand request)
    {
        var staff = request.TestSlot.Staff;
        return staff != null && staff.All(x => x.StaffUserId != Guid.Empty && x.IsActive &&
                (x.RoleId == TestSlotStaffRoleIds.HallSupervisor || x.RoleId == TestSlotStaffRoleIds.Monitor)) &&
            staff.Count(x => x.RoleId == TestSlotStaffRoleIds.HallSupervisor) == 1 &&
            staff.Select(x => x.StaffUserId).Distinct().Count() == staff.Count;
    }
}
