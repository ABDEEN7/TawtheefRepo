using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Lookups.NoneSeeds;

[Table(nameof(Office), Schema = Schemas.Lookup)]
public class Office : LookupBase
{
    public required Guid CountryId { get; init; }
    public Country? Country { get; init; }
    public required Guid OfficeAdminId { get; init; }
    public OfficeUser? OfficeAdmin { get; init; }
    public required string Code { get; init; }
    [MaxLength(10)]
    public string? PhoneCountryCode { get; set; }
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
    public List<OfficeUser>? OfficeUsers { get; init; }
    public List<OfficeSupportedCountry> SupportedCountries { get; init; } = [];
}
