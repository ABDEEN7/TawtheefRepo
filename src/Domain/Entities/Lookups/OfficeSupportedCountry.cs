using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

[Table(nameof(OfficeSupportedCountry), Schema = Schemas.Lookup)]
public class OfficeSupportedCountry: LookupBase
{
    public Guid OfficeId { get; set; }
    public Office Office { get; set; } = default!;

    public Guid CountryId { get; set; }
    public Country Country { get; set; } = default!;
}
