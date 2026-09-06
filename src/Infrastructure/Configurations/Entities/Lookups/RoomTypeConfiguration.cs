using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class RoomTypeConfiguration : LookupBaseConfiguration<RoomType>
{
    public override void Configure(EntityTypeBuilder<RoomType> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new RoomType
            {
                Id = RoomTypeIds.ExamRoom,
                BackendName = nameof(RoomTypeIds.ExamRoom),
                NameEn = "Exam Room",
                NameAr = "غرفة اختبار",
                DisplayOrder = 1
            },
            new RoomType
            {
                Id = RoomTypeIds.InterviewRoom,
                BackendName = nameof(RoomTypeIds.InterviewRoom),
                NameEn = "Interview Room",
                NameAr = "غرفة مقابلة",
                DisplayOrder = 2
            },
            new RoomType
            {
                Id = RoomTypeIds.LabRoom,
                BackendName = nameof(RoomTypeIds.LabRoom),
                NameEn = "Lab Room",
                NameAr = "غرفة مختبر",
                DisplayOrder = 3
            },
            new RoomType
            {
                Id = RoomTypeIds.Other,
                BackendName = nameof(RoomTypeIds.Other),
                NameEn = "Other",
                NameAr = "أخرى",
                DisplayOrder = 4
            }
        );
    }
}

