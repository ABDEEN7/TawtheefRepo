using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;
public static class UniversityIds
{
    public static Guid QatarUniversity = Guid.Parse("33333333-0000-0000-0000-000000000001");
    public static Guid KingSaudUniversity = Guid.Parse("33333333-0000-0000-0000-000000000002");
    public static Guid YarmoukUniversity = Guid.Parse("33333333-0000-0000-0000-000000000003");
}
[Table(nameof(University), Schema = "lkp")]
public class University : LookupBase
{
    public Guid CountryId { get; set; }
    public Country? Country { get; set; }
}
