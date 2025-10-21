using System.ComponentModel.DataAnnotations;
using CSharpFunctionalExtensions;
using MediatR;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record ExternalLinkCommand(
    [AllowedValues("Google", "Outlook", ErrorMessage = "Invalid provider. Supported providers: Google, Outlook")] 
    string Provider, 
    string Token, string? ReturnUrl) : IRequest<Result<LoginResponse>>;
