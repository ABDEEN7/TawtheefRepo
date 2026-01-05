using System.ComponentModel.DataAnnotations;
using Cortex.Mediator.Commands;
using FluentResults;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Common.Models;

public record ExternalLoginRequest(
    [AllowedValues("Google", "Azure", ErrorMessage = "Invalid provider. Supported providers: Google, Azure")] 
    string Provider,
    string? ReturnUrl)
    : ICommand<IResult<AuthResponse>>;
