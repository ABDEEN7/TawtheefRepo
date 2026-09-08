using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class InterviewOrganizationScopeConfiguration : LookupBaseConfiguration<InterviewOrganizationScope>
{
    public override void Configure(EntityTypeBuilder<InterviewOrganizationScope> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new InterviewOrganizationScope
            {
                Id = InterviewOrganizationScopeIds.Ministry,
                BackendName = nameof(InterviewOrganizationScopeIds.Ministry),
                NameEn = "Ministry",
                NameAr = "الوزارة",
                DisplayOrder = 1
            },
            new InterviewOrganizationScope
            {
                Id = InterviewOrganizationScopeIds.Schools,
                BackendName = nameof(InterviewOrganizationScopeIds.Schools),
                NameEn = "Schools",
                NameAr = "المدارس",
                DisplayOrder = 2
            }
        );
    }
}
