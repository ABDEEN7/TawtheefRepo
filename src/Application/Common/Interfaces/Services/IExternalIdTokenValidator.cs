using System.Security.Claims;
using CSharpFunctionalExtensions;

namespace Tawtheef.Application.Common.Interfaces.Services;

public interface IExternalIdTokenValidator
{
    Task<Result<ClaimsPrincipal>> ValidateAsync(string idToken, CancellationToken ct);
}
