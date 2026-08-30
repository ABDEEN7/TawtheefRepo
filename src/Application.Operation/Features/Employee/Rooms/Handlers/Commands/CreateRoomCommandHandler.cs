using Application.Operation.Features.Employee.Rooms.Commands;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Rooms;

namespace Application.Operation.Features.Employee.Rooms.Handlers.Commands;

public sealed class CreateRoomCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateRoomCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(
        CreateRoomCommand request,
        CancellationToken cancellationToken)
    {
        var roomRepository = unitOfWork.GetEntityRepository<Room>();
        var room = request.Room.Adapt<Room>();

        var duplicateExists = await roomRepository.DbSet.AnyAsync(
            existingRoom => existingRoom.Location == room.Location &&
                            (existingRoom.NameAr == room.NameAr || existingRoom.NameEn == room.NameEn),
            cancellationToken);

        if (duplicateExists)
            return Result.Fail<Guid>(ErrorsCodes.RoomNameExistsInLocation);

        await roomRepository.AddAsync(room);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(room.Id);
    }
}
