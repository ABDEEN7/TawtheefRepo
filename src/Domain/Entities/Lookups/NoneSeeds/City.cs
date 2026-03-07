using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups.NoneSeeds;

[Table(nameof(City), Schema = Schemas.Lookup)]
[Index(nameof(CountryId))]
public class City : LookupBase
{
    public Guid CountryId { get; init; }
    public Country? Country { get; init; }
    [MaxLength(50)]
    public required string Code { get; init; }
}
