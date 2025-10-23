using System.ComponentModel.DataAnnotations;
using CSharpFunctionalExtensions;
using MediatR;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record ExternalLoginCommand([AllowedValues("Google", "AzureAD", ErrorMessage = "Invalid provider. Supported providers: Google, AzureAD")] string Provider, string? ReturnUrl) : IRequest<Result<AuthResponse>>;
