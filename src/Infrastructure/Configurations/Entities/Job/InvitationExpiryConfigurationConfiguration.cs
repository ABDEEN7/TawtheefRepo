using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Infrastructure.Configurations.Entities.Job;

public class InvitationExpiryConfigurationConfiguration : IEntityTypeConfiguration<InvitationExpiryConfiguration>
{
    public void Configure(EntityTypeBuilder<InvitationExpiryConfiguration> builder)
    {
        builder.HasData(
            new InvitationExpiryConfiguration
            {
                Id = InvitationExpiryConfigurationIds.Default,
                ExpiryDays = 7,
                CreatedDate = DefaultConfig.DefaultCreatedDate,
                IsDeleted = false,
            }
        );
    }
}
