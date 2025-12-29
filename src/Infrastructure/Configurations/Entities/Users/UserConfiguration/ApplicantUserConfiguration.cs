using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Configurations.Entities.Users.UserConfiguration;

public class ApplicantUserConfiguration : ApplicationUserConfiguration<ApplicantUser>
{
    public override void Configure(EntityTypeBuilder<ApplicantUser> builder)
    {
        base.Configure(builder);
    }
}
