using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class ExamInterruptionPolicyIds
{
    public static readonly Guid ResumeOnly = Guid.Parse("546f8fe0-29ed-4dd2-a7dc-611248258166");
    public static readonly Guid RescheduleOnly = Guid.Parse("2153126f-8d36-4c35-a698-edb984b85829");
    public static readonly Guid ResumeOrReschedule = Guid.Parse("7994a759-00d2-4dc7-806a-dbb311276160");
}

[Table(nameof(ExamInterruptionPolicy), Schema = Schemas.Lookup)]
public class ExamInterruptionPolicy : LookupBase;
