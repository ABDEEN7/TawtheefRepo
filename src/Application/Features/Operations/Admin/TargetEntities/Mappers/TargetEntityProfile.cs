using Mapster;
using Tawtheef.Application.Features.Operations.Admin.TargetEntities.DTOs;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Operations.Admin.TargetEntities.Mappers;

public sealed class TargetEntityProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TargetEntity, TargetEntityAdminDto>();
    }
}
