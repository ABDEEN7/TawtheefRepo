using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class InterviewNoteTypeConfiguration : LookupBaseConfiguration<InterviewNoteType>
{
    public override void Configure(EntityTypeBuilder<InterviewNoteType> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new InterviewNoteType
            {
                Id = InterviewNoteTypeIds.ChairmanNote,
                BackendName = nameof(InterviewNoteTypeIds.ChairmanNote),
                NameEn = "Chairman Note",
                NameAr = "ملاحظة رئيس اللجنة",
                DisplayOrder = 1
            },
            new InterviewNoteType
            {
                Id = InterviewNoteTypeIds.HRNote,
                BackendName = nameof(InterviewNoteTypeIds.HRNote),
                NameEn = "HR Note",
                NameAr = "ملاحظة الموارد البشرية",
                DisplayOrder = 2
            },
            new InterviewNoteType
            {
                Id = InterviewNoteTypeIds.OperationalNote,
                BackendName = nameof(InterviewNoteTypeIds.OperationalNote),
                NameEn = "Operational Note",
                NameAr = "ملاحظة تشغيلية",
                DisplayOrder = 3
            }
        );
    }
}
