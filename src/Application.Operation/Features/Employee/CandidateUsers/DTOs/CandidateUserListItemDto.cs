using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.CandidateUsers.DTOs;

public sealed class CandidateUserListItemDto
{
    public Guid Id { get; init; }
    public string FullNameEn { get; init; } = string.Empty;
    public string FullNameAr { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string MobileNumber { get; init; } = string.Empty;
    public string? Qid { get; init; }
    public bool IsBlocked { get; init; }
    public UserProfileStatus? ProfileStatus { get; init; }
}

