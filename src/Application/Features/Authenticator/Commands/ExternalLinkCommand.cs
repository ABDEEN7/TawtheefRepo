using System.ComponentModel.DataAnnotations;
using CSharpFunctionalExtensions;
using MediatR;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record ExternalLinkCommand(
    [AllowedValues("Google", "AzureAD", ErrorMessage = "Invalid provider. Supported providers: Google, AzureAD")] 
    string Provider, 
    string Token, string? ReturnUrl) : IRequest<Result<AuthResponse>>;
