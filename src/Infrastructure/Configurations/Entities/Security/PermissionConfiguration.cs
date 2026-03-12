using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Security;
using Tawtheef.Infrastructure.Configurations.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Security;

public class PermissionConfiguration : LookupBaseConfiguration<Permission>
{
    public override void Configure(EntityTypeBuilder<Permission> builder)
    {
        base.Configure(builder);

        var seed = PermissionCatalog.All.Select(p => new Permission
        {
            Id = PermissionIds.FromKey(p.Key),
            BackendName = p.Key.Value,
            NameEn = p.NameEn,
            NameAr = p.NameAr,
            DisplayOrder = p.DisplayOrder,
            IsAssignableToRole = p.CanBeAssignedToRole
        });

        builder.HasData(seed);
    }
}
