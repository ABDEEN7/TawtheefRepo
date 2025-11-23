using System.ComponentModel.DataAnnotations;
using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Common.Models;

public record ExternalLoginRequest([AllowedValues("Google", "Azure", ErrorMessage = "Invalid provider. Supported providers: Google, Azure")] string Provider, string? ReturnUrl) : IRequest<IResult<AuthResponse>>;
