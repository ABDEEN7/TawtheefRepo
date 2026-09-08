using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Exams;

namespace Tawtheef.Infrastructure.Configurations.Entities.Exams;

public class ExamCategoryConfiguration : BaseEntityConfiguration<ExamCategory>
{
    public override void Configure(EntityTypeBuilder<ExamCategory> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.ExamPart)
            .WithMany()
            .HasForeignKey(x => x.ExamPartId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.QuestionBankVersion)
            .WithMany()
            .HasForeignKey(x => x.QuestionBankVersionId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

