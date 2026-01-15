using System.Security.Claims;
using FluentResults;

namespace Tawtheef.Application.Common.Interfaces.Services.Security;

public interface IExternalIdTokenValidator
{
    Task<IResult<ClaimsPrincipal>> ValidateAsync(string idToken, CancellationToken ct);
}
