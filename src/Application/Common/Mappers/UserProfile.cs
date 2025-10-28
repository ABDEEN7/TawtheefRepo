using Mapster;
using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Mappers;

public class UserProfile: IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // config.NewConfig<User, DropdownOptions>()
        //     .Map(dest => dest.Name, src => src.DisplayName);
    }
}
