using Mapster;
using Tawtheef.Application.Features.Operations.Admin.Languages.DTOs;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Operations.Admin.Languages.Mappers;

public sealed class LanguageProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Language, LanguageAdminDto>();
    }
}
