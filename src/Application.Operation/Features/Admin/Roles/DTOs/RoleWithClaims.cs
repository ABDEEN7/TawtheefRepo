using System.Security.Claims;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Admin.Roles.DTOs;

internal sealed record RoleWithClaims(ApplicationRole Role, IEnumerable<Claim> Claims);
