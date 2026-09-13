using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class QuestionBankTypeIds
{
    public static readonly Guid Specialized = Guid.Parse("a7f9646b-fcc3-4a00-944c-ceed82acd557");
    public static readonly Guid Skills = Guid.Parse("686928db-b630-4689-b265-ae17eded73cf");
    public static readonly Guid Educational = Guid.Parse("0ef8f0d9-02d7-4863-9bfe-e63355f3686e");
}
[Table(nameof(QuestionBankType), Schema = Schemas.Lookup)]
public class QuestionBankType : LookupBase
{
}
