using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class ExamExemptionDecisionStatusIds
{
    public static readonly Guid Proposed = Guid.Parse("5a79c067-a14d-4c8d-acac-97b965814679");
    public static readonly Guid Approved = Guid.Parse("e902ef1e-7c78-4e44-a978-b40e962a41b8");
    public static readonly Guid Rejected = Guid.Parse("8df71003-be3b-4cdd-a965-9ca3f73a2551");
    public static readonly Guid Cancelled = Guid.Parse("c8bf2d58-54b1-40bc-a1ec-ec7af1427ed0");
}

[Table(nameof(ExamExemptionDecisionStatus), Schema = Schemas.Lookup)]
public class ExamExemptionDecisionStatus : LookupBase;

