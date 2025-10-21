using System;

namespace Tawtheef.Application.Features.Authenticator.DTOs.Responses;

public record RegistrationResponse(
    Guid UserId,
    string Email,
    string UserType,
    bool RequiresAdminApproval
    );