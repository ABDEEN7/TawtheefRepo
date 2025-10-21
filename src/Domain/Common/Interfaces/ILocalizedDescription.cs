namespace Tawtheef.Domain.Common.Interfaces;

public interface ILocalizedDescription
{
    string DescriptionEn { get; set; }
    string DescriptionAr { get; set; }
    
    string GetLocalizedDescription(string language) => language.ToLower() switch
    {
        "ar" => DescriptionAr,
        _ => DescriptionEn
    };
}