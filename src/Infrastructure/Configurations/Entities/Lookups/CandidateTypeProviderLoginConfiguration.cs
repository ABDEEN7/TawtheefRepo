using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class CandidateTypeProviderLoginConfiguration 
    : IEntityTypeConfiguration<CandidateTypeProviderLogin>
{
    public void Configure(EntityTypeBuilder<CandidateTypeProviderLogin> builder)
    {
        builder.HasKey(x => new { x.CandidateTypeId, x.ProviderLoginId });
        builder.HasOne(x => x.CandidateType)
            .WithMany(x => x.CandidateTypeProviderLogins)
            .HasForeignKey(x => x.CandidateTypeId);
        builder.HasOne(x => x.ProviderLogin)
            .WithMany(x => x.CandidateTypeProviderLogins)
            .HasForeignKey(x => x.ProviderLoginId);

        builder.HasData(
            new CandidateTypeProviderLogin
            {
                ProviderLoginId = ProviderLoginIds.QatarPass,
                CandidateTypeId = CandidateTypeIds.Qatari,
            },
            new CandidateTypeProviderLogin
            {
                ProviderLoginId = ProviderLoginIds.QatarPass,
                CandidateTypeId = CandidateTypeIds.SonOfQatariMother,
            },
            new CandidateTypeProviderLogin
            {
                ProviderLoginId = ProviderLoginIds.QatarPass,
                CandidateTypeId = CandidateTypeIds.WifeOfQatari,
            },
            new CandidateTypeProviderLogin
            {
                ProviderLoginId = ProviderLoginIds.QatarPass,
                CandidateTypeId = CandidateTypeIds.ResidentQatar,
            },
            
            new CandidateTypeProviderLogin
            {
                ProviderLoginId = ProviderLoginIds.Google,
                CandidateTypeId = CandidateTypeIds.GCC,
            },
            new CandidateTypeProviderLogin
            {
                ProviderLoginId = ProviderLoginIds.Google,
                CandidateTypeId = CandidateTypeIds.NonQatari,
            }
        );
    }
}
