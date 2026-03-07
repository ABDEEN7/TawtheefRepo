using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Domain.Entities.Lookups;

[Table(nameof(OfficeSupportedCountry), Schema = Schemas.Lookup)]
[Index(nameof(OfficeId))]
[Index(nameof(CountryId))]
public class OfficeSupportedCountry: EventEntity
{
    public Guid OfficeId { get; set; }
    public Office Office { get; set; } = default!;

    public Guid CountryId { get; set; }
    public Country Country { get; set; } = default!;
}