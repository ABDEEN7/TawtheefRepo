using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Exams;

namespace Tawtheef.Infrastructure.Configurations.Entities.Exams;

public class ExamPartConfiguration : BaseEntityConfiguration<ExamPart>
{
    public override void Configure(EntityTypeBuilder<ExamPart> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.Exam)
            .WithMany()
            .HasForeignKey(x => x.ExamId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

