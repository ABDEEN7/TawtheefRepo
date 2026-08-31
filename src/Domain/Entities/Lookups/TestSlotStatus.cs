using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class TestSlotStatusIds
{
    public static readonly Guid Draft = Guid.Parse("02a5a982-e326-403c-a8da-34b2f33d88f0");
    public static readonly Guid Approved = Guid.Parse("7995eaf8-baaf-4dcc-a30d-0becc5a8e4d8");
    public static readonly Guid Ready = Guid.Parse("fd397696-6596-4482-ae56-4c187813a65e");
    public static readonly Guid Started = Guid.Parse("cc71a094-b1b0-4c44-ab10-f48a8cc12140");
    public static readonly Guid Closed = Guid.Parse("46ae567b-7f97-47ef-a960-9fd9efd32a53");
    public static readonly Guid Rescheduled = Guid.Parse("215d0bfd-1ad4-4613-af9e-79d977dfd637");
    public static readonly Guid Cancelled = Guid.Parse("4ff15b36-d258-48f4-ab1c-2380dfc0d1b0");
}

[Table(nameof(TestSlotStatus), Schema = Schemas.Lookup)]
public class TestSlotStatus : LookupBase;

