namespace Tawtheef.Domain.Common.Interfaces;

public interface ILocalizedName
{
    string NameAr { get; set; }
    string NameEn { get; set; }
    string GetLocalizedName(string language) => language.ToLower() switch
    {
        "ar" => NameAr,
        _ => NameEn
    };
}
public interface ILocalizedFullName
{
    string FullNameAr { get; set; }
    string FullNameEn { get; set; }
    string GetLocalizedName(string language) => language.ToLower() switch
    {
        "ar" => FullNameAr,
        _ => FullNameEn
    };
}
