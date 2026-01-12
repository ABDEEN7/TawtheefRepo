using Application.Operation.Features.Admin.Religions.DTOs;
using Mapster;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Admin.Religions.Mappers;

public sealed class ReligionProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Religion, ReligionAdminDto>();
    }
}
