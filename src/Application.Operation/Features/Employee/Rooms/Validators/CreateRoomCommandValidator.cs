using Application.Operation.Features.Employee.Rooms.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.Rooms.Validators;

public sealed class CreateRoomCommandValidator : AbstractValidator<CreateRoomCommand>
{
    public CreateRoomCommandValidator()
    {
        RuleFor(x => x.Room.NameAr)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Room.NameEn)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Room.RoomType)
            .IsInEnum();

        RuleFor(x => x.Room.Capacity)
            .GreaterThan(0);

        RuleFor(x => x.Room.Location)
            .MaximumLength(500);

        RuleFor(x => x.Room.Status)
            .IsInEnum();

        RuleFor(x => x.Room.Notes)
            .MaximumLength(2000);
    }
}
