using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;
public static class RatingGradeIds
{
    public static Guid Excellent = Guid.Parse("4dbd3381-f57a-4f6c-a718-432970d32276");
    public static Guid VeryGood = Guid.Parse("b8f10518-bf02-4357-a0e3-1f6bfb1746cd");
    public static Guid Good = Guid.Parse("2cf3d4e2-33cd-4671-9667-1b5e6e0cee9a");
    public static Guid Acceptable = Guid.Parse("69c0943a-f04f-4b6c-9145-1fbfae4b5c2e");
}
[Table(nameof(RatingGrade), Schema = Schemas.Lookup)]
public class RatingGrade : LookupBase
{
}
