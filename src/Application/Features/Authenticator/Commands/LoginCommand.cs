using CSharpFunctionalExtensions;
using MediatR;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record LoginCommand : IRequest<Result<LoginResponse>>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public string? IpAddress { get; init; }
    
    public string? DeviceId{ get; init; }
    public string? UserAgent{ get; init; }
    public string? Timezone{ get; init; }
    public string? Screen{ get; init; }
    public string? Browser { get; set; }
    public string? DisplayName { get; set; }
    public string? Os { get; set; }
}