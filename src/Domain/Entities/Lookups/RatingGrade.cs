using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;
public static class RatingGradeIds
{
    public static Guid AboveExcellent = Guid.Parse("4dbd3381-f57a-4f6c-a718-432970d32276");
    public static Guid Excellent = Guid.Parse("76bd9c52-9c81-4d8f-afb4-fdd924744082");
    public static Guid VeryGood = Guid.Parse("2cf3d4e2-33cd-4671-9667-1b5e6e0cee9a");
    public static Guid Good = Guid.Parse("71a78988-825a-4a0f-94a3-f59089dbe33d");
    public static Guid Acceptable = Guid.Parse("08e6f782-458e-4334-9bf1-f599c53b437a");
}
[Table(nameof(RatingGrade), Schema = Schemas.Lookup)]
public class RatingGrade : LookupBase
{
}
