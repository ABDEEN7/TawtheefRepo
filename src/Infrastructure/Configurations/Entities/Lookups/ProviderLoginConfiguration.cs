using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class ProviderLoginConfiguration : LookupBaseConfiguration<ProviderLogin>
{
    public override void Configure(EntityTypeBuilder<ProviderLogin> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new ProviderLogin
            {
                Id = ProviderLoginIds.Google,
                BackendName = nameof(ProviderLoginIds.Google),
                NameEn = "Google",
                NameAr = "جوجل",
                DisplayOrder = 1
            },
            new ProviderLogin
            {
                Id = ProviderLoginIds.QatarPass,
                BackendName = nameof(ProviderLoginIds.QatarPass),
                NameEn = "QatarPass",
                NameAr = "قطر باس",
                DisplayOrder = 2
            },
            new ProviderLogin
            {
                Id = ProviderLoginIds.Azure,
                BackendName = nameof(ProviderLoginIds.Azure),
                NameEn = "Azure",
                NameAr = "أزور",
                DisplayOrder = 3
            }
        );
    }
}

