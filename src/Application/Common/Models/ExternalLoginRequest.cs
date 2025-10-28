using System.ComponentModel.DataAnnotations;
using CSharpFunctionalExtensions;
using MediatR;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Common.Models;

public record ExternalLoginRequest([AllowedValues("Google", "AzureAD", ErrorMessage = "Invalid provider. Supported providers: Google, AzureAD")] string Provider, string? ReturnUrl) : IRequest<Result<AuthResponse>>;
