using Application.Operation.Features.Employee.OfficeUsers.DTOs;
using Mapster;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.OfficeUsers.Mappers;

public sealed class OfficeUserProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<OfficeUser, OfficeUserListItemDto>()
            .Map(dest => dest.Email, src => src.Email ?? string.Empty);
    }
}
