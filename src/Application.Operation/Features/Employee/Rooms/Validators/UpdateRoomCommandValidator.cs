using Application.Operation.Features.Employee.Rooms.Commands;
using FluentValidation;

namespace Application.Operation.Features.Employee.Rooms.Validators;

public sealed class UpdateRoomCommandValidator : AbstractValidator<UpdateRoomCommand>
{
    public UpdateRoomCommandValidator()
    {
        RuleFor(x => x.RoomId).NotEmpty();

        RuleFor(x => x.Room.NameAr)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Room.NameEn)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Room.RoomTypeId)
            .NotEmpty();

        RuleFor(x => x.Room.Capacity)
            .GreaterThan(0);

        RuleFor(x => x.Room.LocationId)
            .NotEmpty();

        RuleFor(x => x.Room.StatusId)
            .NotEmpty();

        RuleFor(x => x.Room.Notes)
            .MaximumLength(2000);
    }
}
