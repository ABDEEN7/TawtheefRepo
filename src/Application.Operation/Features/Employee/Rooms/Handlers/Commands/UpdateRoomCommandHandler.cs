using Application.Operation.Features.Employee.Rooms.Commands;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;

namespace Application.Operation.Features.Employee.Rooms.Handlers.Commands;

public sealed class UpdateRoomCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateRoomCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        UpdateRoomCommand request,
        CancellationToken cancellationToken)
    {
        var roomRepository = unitOfWork.GetEntityRepository<Room>();
        var roomResult = await roomRepository.GetByIdAsync(request.RoomId, cancellationToken);

        if (roomResult.IsFailed || roomResult.Value is null)
            return Result.Fail<Unit>(ErrorsCodes.RoomNotFound);

        var room = roomResult.Value;
        var updatedRoom = request.Room.Adapt<Room>();

        var duplicateExists = await roomRepository.DbSet.AnyAsync(
            existingRoom => existingRoom.Id != request.RoomId &&
                            existingRoom.LocationId == updatedRoom.LocationId &&
                            (existingRoom.NameAr == updatedRoom.NameAr ||
                             existingRoom.NameEn == updatedRoom.NameEn),
            cancellationToken);

        if (duplicateExists)
            return Result.Fail<Unit>(ErrorsCodes.RoomNameExistsInLocation);

        request.Room.Adapt(room);

        await roomRepository.UpdateAsync(room, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
