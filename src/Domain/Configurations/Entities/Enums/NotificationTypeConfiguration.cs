using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Configurations.Entities.Enums;

public class NotificationTypeConfiguration : LookupBaseConfiguration<NotificationType>
{
    public override void Configure(EntityTypeBuilder<NotificationType> builder)
    {
        builder.HasData(
            new NotificationType
            {
                Id = NotificationTypeIds.EnrollmentApproved,
                BackendName = nameof(NotificationTypeIds.EnrollmentApproved),
                NameEn = "Enrollment Approved",
                NameAr = "التحاق مقبول",
                DescriptionEn = "Your enrollment has been approved",
                DescriptionAr = "تم قبول التحاقك",
                DisplayOrder = 1
            }
        );
        base.Configure(builder);
    }
}
