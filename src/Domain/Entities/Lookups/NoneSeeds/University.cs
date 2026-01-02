using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups.NoneSeeds;
[Table(nameof(University), Schema = Schemas.Lookup)]
public class University : LookupBase
{
    public Guid CityId { get; set; }
    public City? City { get; set; }

    public string? WebSite { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Code { get; set; }
    public Guid? LogoEnId { get; set; }
    public Resource? LogoEn { get; set; }
    public Guid? LogoArId { get; set; }
    public Resource? LogoAr { get; set; }
    public string? OriginalName { get; set; }
}
