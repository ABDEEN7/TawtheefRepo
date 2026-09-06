using Application.Operation.Features.Employee.Rooms.DTOs;
using Mapster;
using Tawtheef.Domain.Entities.Exams;

namespace Application.Operation.Features.Employee.Rooms.Mappers;

public sealed class RoomProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateRoomDto, Room>()
            .Map(dest => dest.NameAr, src => src.NameAr.Trim())
            .Map(dest => dest.NameEn, src => src.NameEn.Trim())
            .Map(dest => dest.Notes, src => string.IsNullOrWhiteSpace(src.Notes) ? null : src.Notes.Trim());

        config.NewConfig<Room, RoomDto>()
            .Map(dest => dest.LastUpdated, src => src.UpdatedDate ?? src.CreatedDate);

    }
}
