using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Tawtheef.Infrastructure.Configurations.Entities.QuestionsBank;

public class QuestionConfiguration
    : IEntityTypeConfiguration<Question>
{
    public void Configure(
        EntityTypeBuilder<Question> builder)
    {
        builder.Property(x => x.QuestionCode)
            .HasMaxLength(100);
        
        builder.HasIndex(x => x.QuestionCode)
            .IsUnique()
            .HasFilter("[QuestionCode] IS NOT NULL");
    }
}
