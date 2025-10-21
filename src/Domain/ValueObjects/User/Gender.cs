using System;
using CSharpFunctionalExtensions;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.ValueObjects.User;

public static class Gender
{
    public static Result<Guid> TryFrom(string raw)
    {
        if (raw.Equals(nameof(GenderIds.Male), StringComparison.OrdinalIgnoreCase))
            return Result.Success(GenderIds.Male);

        if (raw.Equals(nameof(GenderIds.Female), StringComparison.OrdinalIgnoreCase))
            return Result.Success(GenderIds.Female);

        return Result.Failure<Guid>(DomainErrors.InvalidGender);
    }
}
