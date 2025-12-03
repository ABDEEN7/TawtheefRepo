using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class SkillRequirementTypeIds
{
    public static readonly Guid Essential = Guid.Parse("b0f1f4dd-95ad-3d0b-e74c-684d2d288329");
    public static readonly Guid Optional = Guid.Parse("a014fa55-b3ed-14e4-b4aa-15354a5d1cc3");
}

/// <summary>
/// نوع متطلب المهارة حسب BRD: (أساسية / اختيارية)
/// </summary>
[Table(nameof(SkillRequirementType), Schema = Schemas.Lookup)]
public class SkillRequirementType : LookupBase
{
}
