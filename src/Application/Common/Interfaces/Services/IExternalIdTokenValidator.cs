using System.Security.Claims;
using FluentResults;

namespace Tawtheef.Application.Common.Interfaces.Services;

public interface IExternalIdTokenValidator
{
    Task<IResult<ClaimsPrincipal>> ValidateAsync(string idToken, CancellationToken ct);
}
