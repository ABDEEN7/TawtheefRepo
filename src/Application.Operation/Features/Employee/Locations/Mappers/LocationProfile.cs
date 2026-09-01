using Application.Operation.Features.Employee.Locations.DTOs;
using Mapster;
using Tawtheef.Domain.Entities.Exams;

namespace Application.Operation.Features.Employee.Locations.Mappers;

public sealed class LocationProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Location, LocationDto>();

        config.NewConfig<SaveLocationDto, Location>()
            .Map(dest => dest.NameAr, src => src.NameAr.Trim())
            .Map(
                dest => dest.NameEn,
                src => string.IsNullOrWhiteSpace(src.NameEn) ? null : src.NameEn.Trim())
            .Map(dest => dest.LocationLink, src => src.LocationLink.Trim())
            .Map(
                dest => dest.Notes,
                src => string.IsNullOrWhiteSpace(src.Notes) ? null : src.Notes.Trim());
    }
}
