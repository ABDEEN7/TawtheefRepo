using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Application.Common.Security;

namespace Tawtheef.Infrastructure.Configurations.Entities.Security;

public sealed class ApplicationRoleClaimsConfiguration
    : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> builder)
    {
        // Sort roles and permissions to ensure a stable seeding order across environments and runs.
        var sortedRoles = RolePermissionCatalog.ByRoleId
            .OrderBy(r => r.Key);

        foreach (var (roleId, permissions) in sortedRoles)
        {
            var sortedPermissions = permissions
                .DistinctBy(x => x.Value, StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x.Value, StringComparer.OrdinalIgnoreCase);

            foreach (var perm in sortedPermissions)
            {
                builder.HasData(new IdentityRoleClaim<Guid>
                {
                    // Use a deterministic integer hash derived from RoleId and ClaimValue.
                    // This prevents ID shifts in migrations when the order of catalog changes.
                    Id = GetDeterministicId(roleId, perm.Value),
                    RoleId = roleId,
                    ClaimType = RoleClaimTypes.Permission,
                    ClaimValue = perm.Value
                });
            }
        }
    }

    /// <summary>
    /// Generates a stable, negative integer ID based on a combination of RoleId and Permission.
    /// This ensures that the same Role-Permission mapping always receives the same ID in the seeds.
    /// </summary>
    private static int GetDeterministicId(Guid roleId, string permission)
    {
        var input = $"{roleId:D}_{permission.ToLowerInvariant()}";
        using var sha1 = SHA1.Create();
        var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
        
        // Take the first 4 bytes and convert to a positive int (31-bit to avoid overflow)
        int id = BitConverter.ToInt32(hash, 0) & 0x7FFFFFFF;
        
        // Use negative values to follow the convention for seed data (avoiding collision with auto-inc).
        return -id;
    }
}
