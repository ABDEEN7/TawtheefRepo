using Application.Operation.Features.Admin.Languages.DTOs;
using Mapster;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Admin.Languages.Mappers;

public sealed class LanguageProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Language, LanguageAdminDto>();
    }
}
