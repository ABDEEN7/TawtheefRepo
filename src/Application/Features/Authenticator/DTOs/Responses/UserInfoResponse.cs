using System;

namespace Tawtheef.Application.Features.Authenticator.DTOs.Responses;

public record UserInfoResponse(
    Guid UserId,
    string FullName,
    string Email,
    string? ProfilePictureUrl
);
