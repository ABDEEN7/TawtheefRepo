using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Security;

namespace Tawtheef.Infrastructure.Configurations.Entities.Security;

public sealed class ApplicationRoleClaimsConfiguration
    : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> builder)
    {
        var id = -1;

        foreach ((Guid roleId, IReadOnlyCollection<PermissionKey> permissions) in RolePermissionCatalog.ByRoleId)
        {
            foreach (var perm in permissions.DistinctBy(x => x.Value, StringComparer.OrdinalIgnoreCase))
            {
                builder.HasData(new IdentityRoleClaim<Guid>
                {
                    Id = id--,
                    RoleId = roleId,
                    ClaimType = RoleClaimTypes.Permission,
                    ClaimValue = perm.Value
                });
            }
        }
    }
}
