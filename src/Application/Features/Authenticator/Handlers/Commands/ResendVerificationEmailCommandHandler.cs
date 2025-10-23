using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands;

public class ResendVerificationEmailCommandHandler : IRequestHandler<ResendVerificationEmailCommand, Result<UnverifiedEmailData>>
{
    private readonly IVerificationService _verificationService;
    private readonly UserManager<User> _userManager;

    public ResendVerificationEmailCommandHandler(
        IVerificationService verificationService,
        UserManager<User> userManager)
    {
        _verificationService = verificationService;
        _userManager = userManager;
    }

    public async Task<Result<UnverifiedEmailData>> Handle(ResendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Result.Failure<UnverifiedEmailData>(ErrorsCodes.UserNotFound);

        if (!_verificationService.CanResendEmail(request.Email))
            return Result.Failure<UnverifiedEmailData>(ErrorsCodes.ResendCooldownActive);

        var result = await _verificationService.SendVerificationEmail(request.Email, request.RecipientName);
        return result;
    }
}
