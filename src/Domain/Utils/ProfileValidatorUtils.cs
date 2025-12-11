using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Utils;

public static class ProfileValidatorUtils
{
    public static bool RequiresNationalAddress(Guid candidateTypeId) => IsResidentQatar(candidateTypeId);
    public static bool RequiresOffice(Guid candidateTypeId) => IsResidentOutsideQatar(candidateTypeId);
    public static bool RequiresSponsor(Guid candidateTypeId) => 
        IsResidentQatar(candidateTypeId) && 
        new []
        {
            CandidateTypeIds.ResidentQatar,
            CandidateTypeIds.WifeOfQatari,
        }.Contains(candidateTypeId);
    public static bool IsResidentQatar(Guid candidateTypeId) => 
        new []
        {
            CandidateTypeIds.Qatari,
            CandidateTypeIds.ResidentQatar,
            CandidateTypeIds.SonOfQatariMother,
            CandidateTypeIds.WifeOfQatari,
        }.Contains(candidateTypeId);
    public static bool IsResidentOutsideQatar(Guid candidateTypeId) => 
        new []
        {
            CandidateTypeIds.GCC,
            CandidateTypeIds.NonQatari,
        }.Contains(candidateTypeId);
    public static bool RequiresBirthCertificate(Guid candidateTypeId) => candidateTypeId == CandidateTypeIds.SonOfQatariMother;
    public static bool RequiresMarriageCertificate(Guid candidateTypeId) => candidateTypeId == CandidateTypeIds.WifeOfQatari;
}
