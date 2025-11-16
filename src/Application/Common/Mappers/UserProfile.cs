using Mapster;

namespace Tawtheef.Application.Common.Mappers;

public class UserProfile: IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // config.NewConfig<User, DropdownOptions>()
        //     .Map(dest => dest.Name, src => src.DisplayName);
    }
}
