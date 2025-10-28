using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Configurations.Entities.Users.UserConfiguration;

public class UserConfiguration : ApplicationUserConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);
    }
}
