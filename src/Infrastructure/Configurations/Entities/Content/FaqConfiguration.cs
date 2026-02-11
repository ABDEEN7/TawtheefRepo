using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Content;
using Tawtheef.Infrastructure.Configurations.Entities;

namespace Tawtheef.Infrastructure.Configurations.Entities.Content;

public sealed class FaqConfiguration : BaseEntityConfiguration<FAQ>
{
    public override void Configure(EntityTypeBuilder<FAQ> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.QuestionAr).IsRequired().HasMaxLength(400);
        builder.Property(x => x.QuestionEn).IsRequired().HasMaxLength(400);
        builder.Property(x => x.AnswerAr).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.AnswerEn).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.DisplayOrder).HasDefaultValue(0);
        builder.Property(x => x.IsActive).HasDefaultValue(true);

        builder.HasIndex(x => x.DisplayOrder);
        builder.HasIndex(x => x.IsActive);
    }
}
