using FluentValidation;
using Tawtheef.Application.Features.Authenticator.DTOs;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public sealed class AzureExternalCallbackLoginCommandValidator : AbstractValidator<AzureExternalCallbackLoginCommand>
{
    private static readonly string[] Allowed =
        ["azure", "azuread", "microsoft"];

    public AzureExternalCallbackLoginCommandValidator()
    {
        RuleFor(x => x.Provider)
            .NotEmpty()
            .Must(p => Allowed.Contains(p.Trim().ToLowerInvariant()))
            .WithMessage(ErrorsCodes.ExternalLoginProviderNotSupported);

        RuleFor(x => x.IdToken)
            .NotEmpty().WithMessage(ErrorsCodes.ExternalLoginInvalidToken);
    }
}
