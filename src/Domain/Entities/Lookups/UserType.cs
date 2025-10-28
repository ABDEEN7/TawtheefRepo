using System;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class UserTypeIds
{
    public static readonly Guid Employee = Guid.Parse("a1b2c3d4-e5f6-4879-8a3b-5c6d7e8f9a0b");
    public static readonly Guid Applicant = Guid.Parse("b2c3d4e5-f6a7-5984-9b2c-6d7e8f9a0b1c");
    public static readonly Guid Admin = Guid.Parse("c3d4e5f6-a7b8-6a95-0c3d-7e8f9a0b1c2d");
}
[Table(nameof(UserType), Schema = "lkp")]
public class UserType : LookupBase
{
}
