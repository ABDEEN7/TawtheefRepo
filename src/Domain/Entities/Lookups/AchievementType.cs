using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class AchievementTypeIds
{
    public static readonly Guid Certificate = Guid.Parse("3f5f4d1f-b214-44cb-9b9e-2f9ec3c1f7f1");
    public static readonly Guid Award = Guid.Parse("6c3b1b1b-0c9c-4e0e-8d1e-4d6c12e91533");
}

[Table(nameof(AchievementType), Schema = Schemas.Lookup)]
public class AchievementType : LookupBase;
