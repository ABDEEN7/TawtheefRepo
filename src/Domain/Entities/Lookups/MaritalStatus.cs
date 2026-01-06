using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class MaritalStatusIds
{
    public static Guid Single = Guid.Parse("c28ab4a0-59b0-4a64-82c4-ba1bd89efaba");
    public static Guid Married = Guid.Parse("7113eb36-44b8-457a-96e9-61bfe6a06f05"); 
    public static Guid Divorced = Guid.Parse("18e83653-978c-44c8-8691-43f95d9a5b7d");
    public static Guid Widowed = Guid.Parse("8f22e74f-672b-47f4-8f32-93f9dbcb15da");
}
[Table(nameof(MaritalStatus), Schema = Schemas.Lookup)]
public class MaritalStatus : LookupBase;
