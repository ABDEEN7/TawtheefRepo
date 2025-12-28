using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Configurations.Entities.Users.UserConfiguration;

public class EmployeeUserConfiguration : ApplicationUserConfiguration<EmployeeUser>
{
    public override void Configure(EntityTypeBuilder<EmployeeUser> builder)
    {
        base.Configure(builder);
    }
}
