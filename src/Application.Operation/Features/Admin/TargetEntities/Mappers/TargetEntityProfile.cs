using Application.Operation.Features.Admin.TargetEntities.DTOs;
using Mapster;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Admin.TargetEntities.Mappers;

public sealed class TargetEntityProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TargetEntity, TargetEntityAdminDto>();
    }
}
