using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Tawtheef.Application.Features.Authenticator.DTOs;
[DataContract(
    Name = "ApiResponseOfPersonalInfoViewModelY_SsWVfMV",
    Namespace = "http://schemas.datacontract.org/2004/07/MOI.NEW.Services.Models")]
public class PersonalInfoApiResponse
{
    [DataMember]
    public bool IsSuccess { get; set; }

    [DataMember]
    public string? Message { get; set; }

    [DataMember]
    public MOEPersonalInfo? ResponseData { get; set; }
}

[DataContract(Name = "PersonalInfoViewModel", 
    Namespace = "http://schemas.datacontract.org/2004/07/MOI.NEW.Services.Models")]
public class MOEPersonalInfo
{
    [DataMember]
    public required string Qid { get; set; }

    // Arabic
    [DataMember(Name = "ArabicName1")]
    public required string ArabicFirstName { get; set; }

    [DataMember(Name = "ArabicName2")]
    public string? ArabicSecondName { get; set; }

    [DataMember(Name = "ArabicName3")]
    public string? ArabicThirdName { get; set; }

    [DataMember(Name = "ArabicName4")]
    public string? ArabicFourthName { get; set; }

    [DataMember(Name = "ArabicName5")]
    public required string ArabicFamilyName { get; set; }

    // English
    [DataMember(Name = "EnglishName1")]
    public required string EnglishFirstName { get; set; }

    [DataMember(Name = "EnglishName2")]
    public string? EnglishSecondName { get; set; }

    [DataMember(Name = "EnglishName3")]
    public string? EnglishThirdName { get; set; }

    [DataMember(Name = "EnglishName4")]
    public string? EnglishFourthName { get; set; }

    [DataMember(Name = "EnglishName5")]
    public required string EnglishFamilyName { get; set; }

    [DataMember]
    public DateOnly DateOfBirth { get; set; }

    [DataMember]
    public DateOnly QIDExpiry { get; set; }

    [DataMember]
    public int NationalityCode { get; set; }

    [DataMember]
    public required string NationalityNameArabic { get; set; }

    [DataMember]
    public required string NationalityNameEnglish { get; set; }

    [DataMember]
    public required string PersonType { get; set; }

    [DataMember]
    public DateOnly ResidencyExpiryDate { get; set; }

    [DataMember]
    public DateOnly ResidencyIssueDate { get; set; }

    [DataMember]
    [AllowedValues("MALE", "FEMALE", ErrorMessage = "Gender must be either 'MALE' or 'FEMALE'.")]
    public required string Gender { get; set; }

    [DataMember]
    public string? Status { get; set; }

    [DataMember]
    public DateOnly StatusDate { get; set; }

    // ===== Helpers (لا تُرسل في الـ XML) =====

    [IgnoreDataMember]
    public string ArabicFullName =>
        string.Join(" ", new[]
        {
            ArabicFirstName,
            ArabicSecondName,
            ArabicThirdName,
            ArabicFourthName,
            ArabicFamilyName
        }.Where(x => !string.IsNullOrWhiteSpace(x)));

    [IgnoreDataMember]
    public string EnglishFullName =>
        string.Join(" ", new[]
        {
            EnglishFirstName,
            EnglishSecondName,
            EnglishThirdName,
            EnglishFourthName,
            EnglishFamilyName
        }.Where(x => !string.IsNullOrWhiteSpace(x)));
}
