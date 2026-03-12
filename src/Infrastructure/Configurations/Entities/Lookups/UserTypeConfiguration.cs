using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class UserTypeConfiguration : LookupBaseConfiguration<UserType>
{
    public override void Configure(EntityTypeBuilder<UserType> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new UserType
            {
                Id = UserTypeIds.Employee,
                BackendName = nameof(UserTypeIds.Employee),
                NameEn = "Employee",
                NameAr = "موظف",
                DisplayOrder = 1
            },
            new UserType
            {
                Id = UserTypeIds.Applicant,
                BackendName = nameof(UserTypeIds.Applicant),
                NameEn = "Applicant",
                NameAr = "متقدم",
                DisplayOrder = 2
            },
            new UserType
            {
                Id = UserTypeIds.OfficeUser,
                BackendName = nameof(UserTypeIds.OfficeUser),
                NameEn = "OfficeUser",
                NameAr = "موظف مكتب",
                DisplayOrder = 4
            }
        );
    }
}
