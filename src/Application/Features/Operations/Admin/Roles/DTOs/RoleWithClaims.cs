using System.Security.Claims;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.DTOs;

internal sealed record RoleWithClaims(ApplicationRole Role, IEnumerable<Claim> Claims);
