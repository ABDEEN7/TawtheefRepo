using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Lookups.NoneSeeds;

[Table(nameof(Office), Schema = Schemas.Lookup)]
public class Office : LookupBase
{
    public Guid CountryId { get; set; }
    public Country? Country { get; set; }
    public List<OfficeSupportedCountry> SupportedCountries { get; set; } = [];
    public List<OfficeUser>? OfficeUsers { get; set; }
    public required string Code { get; set; }
}
