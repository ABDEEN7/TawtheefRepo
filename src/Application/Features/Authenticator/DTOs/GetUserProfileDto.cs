using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.DTOs;

public class GetUserProfileDto
{
    public Guid UserId { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Avatar { get; set; }
    public EmployeeProfileDto? Profile { get; set; }
}

public class EmployeeProfileDto
{
    public string? EmployeeNumber { get; set; }
    public string? Qid { get; set; }
    public string? FullNameAr { get; set; }
    public string? FullNameEn { get; set; }
    public string? Email { get; set; }
    public string? MobileNumber { get; set; }
    public string? Department { get; set; }
    public string? DepartmentNumber { get; set; }
    public string? Section { get; set; }
    public string? SectionNumber { get; set; }
    public string? JobTitle { get; set; }
}
