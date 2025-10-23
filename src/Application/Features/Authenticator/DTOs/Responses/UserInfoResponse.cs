using System;

namespace Tawtheef.Application.Features.Authenticator.DTOs.Responses;

public record UserInfoResponse(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? ProfilePictureUrl
);
