using Mapster;
using Tawtheef.Application.Extensions;

namespace Tawtheef.Application.Common.Mappers;

public class DateTimeMapper : IRegister
{
    /*
     * This is a known Mapster gotcha, and it comes down to two things interacting badly:

    *  1. DateTimeOffset has a built-in implicit conversion operator from DateTime.
    *  
    *  Mapster detects that operator and, for member-by-member auto-mapping (i.e. members you don't explicitly .Map()),
     * it prefers using that native implicit conversion over your registered
     * TypeAdapterConfig<DateTime?, DateTimeOffset?> rule. The codegen it produces for that "implicit operator" path
     * doesn't reliably null-check the Nullable<DateTime> the way your MapWith(src => src.HasValue ? ... : null)
     * rule does — it ends up compiling something that touches .Value on the nullable without a guard, which
     * throws InvalidOperationException: Nullable object must have a value the moment PublishAt is null
     * (which it will be for any unpublished job).
    *  
    *  2. Registration/compile ordering.
    *  
    *  Your DateTimeMapper : IRegister global rule for DateTime? → DateTimeOffset? is correct and would work
     * if Mapster consistently routed nested member conversions through it. But when a destination member is left to
     * auto-map (no explicit .Map() for it), Mapster resolves the conversion at config-build time per type pair,
     * and in practice that resolution doesn't reliably fall back to your global nullable rule when a native implicit
     * operator exists on the target type — it just uses the operator path instead, ignoring your safer custom logic.
    *  
    *  That's exactly why uncommenting:
    *  .Map(dest => dest.PublishAt, src => src.PublishAt.HasValue ? src.PublishAt.Value.AsUtcOffset() : (DateTimeOffset?)null)
    *  
    *  fixes it — you're forcing Mapster to use your explicit, null-safe expression for that specific member instead
     * of letting it silently pick the implicit-operator auto-map path.
     */
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DateTime, DateTimeOffset>()
            .MapWith(src => src.AsUtcOffset());
        config.NewConfig<DateTime?, DateTimeOffset?>()
            .MapWith(src => src.HasValue ? src.Value.AsUtcOffset() : null);
        
        config.NewConfig<DateTimeOffset?, DateTime?>()
            .MapWith(src => src.HasValue ? src.Value.UtcDateTime : null);
        config.NewConfig<DateTimeOffset, DateTime>()
            .MapWith(src => src.UtcDateTime);
    }
}
