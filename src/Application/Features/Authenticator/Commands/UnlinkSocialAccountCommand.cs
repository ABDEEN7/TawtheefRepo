using System;
using System.ComponentModel.DataAnnotations;
using CSharpFunctionalExtensions;
using MediatR;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record UnlinkSocialAccountCommand(Guid UserId,[AllowedValues("Google", "Outlook", ErrorMessage = "Invalid provider. Supported providers: Google, Outlook")] string Provider) : IRequest<Result>;
