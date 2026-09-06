using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class ExamCategoryTypeIds
{
    public static readonly Guid Specialized = Guid.Parse("e7c03fb9-c9a3-4187-a731-88873a230983");
    public static readonly Guid Educational = Guid.Parse("6b165f67-6da7-4715-a815-e9f735704b49");
    public static readonly Guid Skills = Guid.Parse("6552e6b6-b339-4476-ab86-60f225ac3582");
}

[Table(nameof(ExamCategoryType), Schema = Schemas.Lookup)]
public class ExamCategoryType : LookupBase;

