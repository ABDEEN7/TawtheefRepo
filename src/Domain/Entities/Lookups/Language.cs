using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;
public static class LanguageIds
{
    public static Guid Arabic = Guid.Parse("9843b692-ef7c-44a7-b1e8-1dcf2b6d87dd");
    public static Guid English = Guid.Parse("8672c4c2-f635-4d26-843a-223fba3a6322");
}

[Table(nameof(Language), Schema = "lkp")]
public class Language : LookupBase
{
}
