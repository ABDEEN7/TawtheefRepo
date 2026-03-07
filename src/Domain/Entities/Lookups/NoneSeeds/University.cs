using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups.NoneSeeds;
[Table(nameof(University), Schema = Schemas.Lookup)]
[Index(nameof(CityId))]
[Index(nameof(LogoEnId))]
[Index(nameof(LogoArId))]
public class University : LookupBase
{
    public Guid CityId { get; set; }
    public City? City { get; set; }

    // URLs can be long but should still be capped
    [MaxLength(2048)]
    public string? WebSite { get; set; }

    // E.164 max = 15 digits, plus symbols (+, -, spaces)
    [MaxLength(20)]
    public string? Phone { get; set; }

    // RFC-compliant emails max length = 254
    [MaxLength(254)]
    public string? Email { get; set; }

    // Internal / external university code
    [MaxLength(50)]
    public string? Code { get; set; }

    public Guid? LogoEnId { get; set; }
    public Resource? LogoEn { get; set; }

    public Guid? LogoArId { get; set; }
    public Resource? LogoAr { get; set; }

    // Official native-language name
    [MaxLength(256)]
    public string? OriginalName { get; set; }
}

