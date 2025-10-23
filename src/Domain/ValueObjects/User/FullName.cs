using System;
using CSharpFunctionalExtensions;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.ValueObjects.User;

public sealed record FullName(string First, string Last)
{
    public static Result<FullName> TryParse(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) 
            return Result.Failure<FullName>(DomainErrors.InvalidName);

        var parts = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length == 2
            ? Result.Success(new FullName(parts[0], parts[1]))
            : Result.Failure<FullName>(DomainErrors.InvalidName);
    }
}
