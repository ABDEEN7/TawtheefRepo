using Application.Operation.Features.Admin.HomeContent.SuccessStories.DTOs;
using Mapster;
using Tawtheef.Domain.Entities.Content;

namespace Application.Operation.Features.Admin.HomeContent.SuccessStories.Mappers;

public sealed class HomeSuccessStoryProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<HomeSuccessStory, HomeSuccessStoryAdminDto>();
    }
}
