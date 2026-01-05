using Mapster;
using Tawtheef.Application.Features.Operations.Admin.Religions.DTOs;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Operations.Admin.Religions.Mappers;

public sealed class ReligionProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Religion, ReligionAdminDto>();
    }
}
