namespace Tawtheef.Application.Features.Authenticator.DTOs;

public class ExternalTokenDto
{
    public required string Provider { get; set; }
    public required string IdToken { get; set; }
}
