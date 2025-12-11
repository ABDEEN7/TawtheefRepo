using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;
public static class LanguageLevelIds
{
    public static Guid Basic = Guid.Parse("f0c9e84a-258f-4f6b-a469-153a92d68730");
    public static Guid Intermediate = Guid.Parse("afd6d7b4-9e20-40bc-a571-0df1670761b0");
    public static Guid Advanced = Guid.Parse("9aa2c1bc-2040-40e1-86a2-72cac90928e1");
    public static Guid Expert = Guid.Parse("5161f23a-f501-414c-b4fe-81cde9ebcd5d");
    public static Guid Native = Guid.Parse("c7d8b159-c9dc-48a1-be6d-26493423f8a9");
}
[Table(nameof(LanguageLevel), Schema = Schemas.Lookup)]
public class LanguageLevel : LookupBase
{
}
public static class SkillLevelIds
{
    public static Guid Expert = Guid.Parse("4dbd3381-f57a-4f6c-a718-432970d32276");
    public static Guid Advanced = Guid.Parse("b8f10518-bf02-4357-a0e3-1f6bfb1746cd");
    public static Guid Intermediate = Guid.Parse("2cf3d4e2-33cd-4671-9667-1b5e6e0cee9a");
    public static Guid Basic = Guid.Parse("69c0943a-f04f-4b6c-9145-1fbfae4b5c2e");
}
[Table(nameof(SkillLevel), Schema = Schemas.Lookup)]
public class SkillLevel : LookupBase
{
}
