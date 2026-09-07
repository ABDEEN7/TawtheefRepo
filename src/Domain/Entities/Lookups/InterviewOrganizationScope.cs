using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class InterviewOrganizationScopeIds
{
    public static Guid Ministry = Guid.Parse("8e6f2a11-6a1e-4b8a-9f0a-1a2b3c4d5e01");
    public static Guid Schools = Guid.Parse("8e6f2a11-6a1e-4b8a-9f0a-1a2b3c4d5e02");
}

[Table(nameof(InterviewOrganizationScope), Schema = Schemas.Lookup)]
public class InterviewOrganizationScope : LookupBase;
