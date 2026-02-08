using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Content;
using Tawtheef.Infrastructure.Configurations.Entities;

namespace Tawtheef.Infrastructure.Configurations.Entities.Content;

public sealed class HomeSuccessStoryConfiguration : BaseEntityConfiguration<HomeSuccessStory>
{
    public override void Configure(EntityTypeBuilder<HomeSuccessStory> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.NameAr).IsRequired().HasMaxLength(200);
        builder.Property(x => x.NameEn).IsRequired().HasMaxLength(200);
        builder.Property(x => x.RoleAr).IsRequired().HasMaxLength(200);
        builder.Property(x => x.RoleEn).IsRequired().HasMaxLength(200);
        builder.Property(x => x.MetricTitleAr).IsRequired().HasMaxLength(200);
        builder.Property(x => x.MetricTitleEn).IsRequired().HasMaxLength(200);
        builder.Property(x => x.MetricDescriptionAr).IsRequired().HasMaxLength(400);
        builder.Property(x => x.MetricDescriptionEn).IsRequired().HasMaxLength(400);
        builder.Property(x => x.ImageUrl).IsRequired().HasMaxLength(2048);
        builder.Property(x => x.DisplayOrder).HasDefaultValue(0);
        builder.Property(x => x.IsActive).HasDefaultValue(true);

        builder.HasIndex(x => x.DisplayOrder);
        builder.HasIndex(x => x.IsActive);
    }
}
