using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Utils;

public static class ProfileValidatorUtils
{
    public static bool RequiresNationalAddress(Guid? candidateTypeId, string provider) => IsResidentQatar(candidateTypeId, provider);
    public static bool RequiresOffice(Guid? candidateTypeId, string provider) => IsResidentOutsideQatar(candidateTypeId, provider);
    public static bool RequiresSponsor(Guid? candidateTypeId, string provider) => 
        IsResidentQatar(candidateTypeId, provider) && 
        new []
        {
            CandidateTypeIds.ResidentQatar,
            CandidateTypeIds.WifeOfQatari,
        }.Contains(candidateTypeId ?? Guid.Empty);
    public static bool IsResidentQatar(Guid? candidateTypeId, string provider) => 
        new []
        {
            CandidateTypeIds.Qatari,
            CandidateTypeIds.ResidentQatar,
            CandidateTypeIds.SonOfQatariMother,
            CandidateTypeIds.WifeOfQatari,
        }.Contains(candidateTypeId ?? Guid.Empty) || (candidateTypeId == CandidateTypeIds.GCC && provider == nameof(ProviderLoginIds.QatarPass));
    public static bool IsResidentOutsideQatar(Guid? candidateTypeId, string provider) => 
        !IsResidentQatar(candidateTypeId, provider);
    public static bool RequiresBirthCertificate(Guid? candidateTypeId) => candidateTypeId == CandidateTypeIds.SonOfQatariMother;
    public static bool RequiresMarriageCertificate(Guid? candidateTypeId) => candidateTypeId == CandidateTypeIds.WifeOfQatari;
}
