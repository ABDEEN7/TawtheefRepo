using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Configurations.Entities.Enums;

public class UserTypeConfiguration : LookupBaseConfiguration<UserType>
{
    public override void Configure(EntityTypeBuilder<UserType> builder)
    {
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
                Id = UserTypeIds.Admin,
                BackendName = nameof(UserTypeIds.Admin),
                NameEn = "Administrator",
                NameAr = "مسؤول النظام",
                DisplayOrder = 3
            }
        );
        base.Configure(builder);
    }
}
