using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.DTOs;

internal sealed record RoleWithClaims(IdentityRole<Guid> Role, IEnumerable<Claim> Claims);
