using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Tawtheef.Infrastructure.Configurations.Entities.QuestionsBank;

public class QuestionRevisionOptionConfiguration
    : IEntityTypeConfiguration<QuestionRevisionOption>
{
    public void Configure(
        EntityTypeBuilder<QuestionRevisionOption> builder)
    {
        builder.HasIndex(x => new
            {
                x.QuestionRevisionId,
                x.DisplayOrder
            })
            .IsUnique();

        builder.HasOne(x => x.QuestionRevision)
            .WithMany(x => x.Options)
            .HasForeignKey(x => x.QuestionRevisionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
