using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.ValueObjects.User;

namespace Tawtheef.Domain.Entities.Users;

public static class EmployeeSuperAdminIds
{
    public static readonly Guid EmployeeId1 = new("200c5018-fa8c-4ee7-a088-9077200b125c");
    public static readonly Guid EmployeeId2 = new("781561c3-0175-4165-80c1-7c6a79130b25");
    public static readonly Guid EmployeeId3 = new("207bd05b-ebd8-4cea-80ad-38fe2479eac6");
    public static readonly Guid EmployeeId4 = new("9fd109fb-a2ad-4637-86f2-2be13ccfa6d4");
}

[Index(nameof(EmployeeProfileId))]
public class EmployeeUser : User
{
    public Guid? EmployeeProfileId { get; init; }
    public EmployeeProfile? EmployeeProfile { get; set; }
    public static Result<User> Register(string email,string displayName)
    {
        var name = FullName.TryParse(displayName);
        if (name.IsFailed) return Result.Fail<User>(name.Errors);

        var userId = Guid.NewGuid();
        var user = new EmployeeUser
        {
            Id = userId,
            Email = email,
            UserName = email,
            FullNameEn = name.Value.First + " " + name.Value.Last,
            FullNameAr = name.Value.First + " " + name.Value.Last,
            UserTypeId = UserTypeIds.Employee
        };
        return Result.Ok<User>(user);
    }
}
