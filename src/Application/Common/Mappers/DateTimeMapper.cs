using Mapster;
using Tawtheef.Application.Extensions;

namespace Tawtheef.Application.Common.Mappers;

public class DateTimeMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DateTime, DateTimeOffset>()
            .MapWith(src => src.AsUtcOffset());

        config.NewConfig<DateTime?, DateTimeOffset?>()
            .MapWith(src => src.AsUtcOffset());
        
        config.NewConfig<DateTimeOffset?, DateTime?>()
            .MapWith(src => src.HasValue ? src.Value.UtcDateTime : null);
        config.NewConfig<DateTimeOffset, DateTime>()
            .MapWith(src => src.UtcDateTime);
    }
}
