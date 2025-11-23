using FluentResults;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.ValueObjects.User;

public static class Gender
{
    public static Result<Guid> TryFrom(string raw)
    {
        if (raw.Equals(nameof(GenderIds.Male), StringComparison.OrdinalIgnoreCase))
            return Result.Ok(GenderIds.Male);

        if (raw.Equals(nameof(GenderIds.Female), StringComparison.OrdinalIgnoreCase))
            return Result.Ok(GenderIds.Female);

        return Result.Fail<Guid>(DomainErrors.InvalidGender);
    }
}
