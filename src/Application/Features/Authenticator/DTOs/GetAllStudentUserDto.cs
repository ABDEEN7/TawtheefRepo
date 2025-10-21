using System;

namespace Tawtheef.Application.Features.Authenticator.DTOs;

public record GetAllStudentUserDto(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string UserType,
    string AccountStatus,
    string? ProfilePictureUrl,
    DateTime? LastLoginDate
);
