namespace Application.Operation.Features.Authenticator.DTOs;

public class GetOperationProfileDto
{
    public Guid UserId { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Avatar { get; set; }
    public EmployeeProfileDto? Profile { get; set; }
}
