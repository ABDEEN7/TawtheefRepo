using Application.Operation.Features.Admin.HomeContent.Faqs.DTOs;
using Mapster;
using Tawtheef.Domain.Entities.Content;

namespace Application.Operation.Features.Admin.HomeContent.Faqs.Mappers;

public sealed class FaqProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<FAQ, FAQAdminDto>();
    }
}
