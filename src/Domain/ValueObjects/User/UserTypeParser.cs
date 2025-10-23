using System;
using CSharpFunctionalExtensions;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.ValueObjects.User;

public static class UserTypeParser
{
    public static Result<Guid> TryFrom(string raw)
    {
        if (raw.Equals(nameof(UserTypeIds.Applicant), StringComparison.OrdinalIgnoreCase))
            return Result.Success(UserTypeIds.Applicant);

        if (raw.Equals(nameof(UserTypeIds.Employee), StringComparison.OrdinalIgnoreCase))
            return Result.Success(UserTypeIds.Employee);

        return Result.Failure<Guid>(DomainErrors.InvalidUserType);
    }
}
