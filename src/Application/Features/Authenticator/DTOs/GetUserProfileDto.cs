using System;

namespace Tawtheef.Application.Features.Authenticator.DTOs;

public class GetUserProfileDto
{
    public Guid UserId { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Avatar { get; set; }
}