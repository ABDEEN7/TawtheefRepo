using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class ExamInterruptionPolicyConfiguration : LookupBaseConfiguration<ExamInterruptionPolicy>
{
    public override void Configure(EntityTypeBuilder<ExamInterruptionPolicy> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new ExamInterruptionPolicy
            {
                Id = ExamInterruptionPolicyIds.ResumeOnly,
                BackendName = nameof(ExamInterruptionPolicyIds.ResumeOnly),
                NameEn = "Resume Only",
                NameAr = "الاستئناف فقط",
                DisplayOrder = 1,
                IsActive = true
            },
            new ExamInterruptionPolicy
            {
                Id = ExamInterruptionPolicyIds.RescheduleOnly,
                BackendName = nameof(ExamInterruptionPolicyIds.RescheduleOnly),
                NameEn = "Reschedule Only",
                NameAr = "إعادة الجدولة فقط",
                DisplayOrder = 2,
                IsActive = true
            },
            new ExamInterruptionPolicy
            {
                Id = ExamInterruptionPolicyIds.ResumeOrReschedule,
                BackendName = nameof(ExamInterruptionPolicyIds.ResumeOrReschedule),
                NameEn = "Resume or Reschedule",
                NameAr = "الاستئناف أو إعادة الجدولة",
                DisplayOrder = 3,
                IsActive = true
            }
        );
    }
}
