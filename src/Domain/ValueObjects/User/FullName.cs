using FluentResults;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.ValueObjects.User;

public sealed record FullName(string First, string Last)
{
    public static Result<FullName> TryParse(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) 
            return Result.Fail<FullName>(DomainErrors.InvalidName);

        var parts = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length == 2
            ? Result.Ok(new FullName(parts[0], parts[1]))
            : Result.Fail<FullName>(DomainErrors.InvalidName);
    }
}
