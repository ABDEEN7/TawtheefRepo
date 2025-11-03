using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class JobCategoryIds
{
    public static Guid Academic = Guid.Parse("62ecfbcc-7ae7-45c0-9db4-fd50751a336e");
    public static Guid Administrative = Guid.Parse("4e7c8fe7-475b-4e4d-99f0-cfef85020d5b");
    public static Guid Labor = Guid.Parse("3d31e01b-9c57-4b47-8584-9fa8949838e8");
    
}
[Table(nameof(JobCategory), Schema = Schemas.Lookup)]
public class JobCategory : LookupBase
{
}
