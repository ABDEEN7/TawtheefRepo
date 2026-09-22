namespace Application.Operation.Features.Employee.TestSlots.DTOs;

public sealed record TestSlotStaffMemberDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public string? JobTitle { get; init; }
    public string? DepartmentName { get; init; }
    public bool IsBlocked { get; init; }
}
