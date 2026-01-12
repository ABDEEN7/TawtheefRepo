using Mapster;

namespace Tawtheef.Application.Common.Mappers;

public class DateTimeMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DateTime, DateTimeOffset>()
            .MapWith(src => new DateTimeOffset(src));

        config.NewConfig<DateTime?, DateTimeOffset?>()
            .MapWith(src => src.HasValue ? new DateTimeOffset(src.Value) : null);
        
        config.NewConfig<DateTimeOffset?, DateTime?>()
            .MapWith(src => src.HasValue ? src.Value.UtcDateTime : null);
        config.NewConfig<DateTimeOffset, DateTime>()
            .MapWith(src => src.UtcDateTime);
    }
}
