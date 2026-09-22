using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class TestSlotStatusIds
{
    public static readonly Guid Ready = Guid.Parse("fd397696-6596-4482-ae56-4c187813a65e");
    public static readonly Guid Started = Guid.Parse("cc71a094-b1b0-4c44-ab10-f48a8cc12140");
    public static readonly Guid Closed = Guid.Parse("46ae567b-7f97-47ef-a960-9fd9efd32a53");
    public static readonly Guid Rescheduled = Guid.Parse("215d0bfd-1ad4-4613-af9e-79d977dfd637");
}

[Table(nameof(TestSlotStatus), Schema = Schemas.Lookup)]
public class TestSlotStatus : LookupBase;

