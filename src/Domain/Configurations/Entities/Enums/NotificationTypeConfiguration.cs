using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Configurations.Entities.Enums;

public class NotificationTypeConfiguration : LookupBaseConfiguration<NotificationType>
{
    public override void Configure(EntityTypeBuilder<NotificationType> builder)
    {
        builder.HasData();
        base.Configure(builder);
    }
}
