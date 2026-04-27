using System.Security.Cryptography;
using System.Text.Json;
using Application.Recruitment.Features.Authenticator.Commands.Verification;
using Application.Recruitment.Features.Authenticator.Handlers.Commands.QatarResidentOtp;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Notifications.Templates.PhoneVerificationCode;

namespace Application.Recruitment.Features.Authenticator.Handlers.Commands.VerificationHandler;


public class RequestPhoneVerificationCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    UserManager<User> userManager)
    : IRequestHandler<RequestPhoneVerificationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(RequestPhoneVerificationCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null)
            return Result.Fail<Unit>(ErrorsCodes.UserNotFound);
        
        var phoneAlreadyUsed = await userManager.Users
            .AnyAsync(u => u.PhoneNumber == request.PhoneE164 && u.Id != request.UserId, cancellationToken);
        if (phoneAlreadyUsed)
            return Result.Fail<Unit>(ErrorsCodes.PhoneAlreadyInUse);
        
        var oneMinuteAgo = timeProvider.GetUtcNow().AddMinutes(-1);
        var lastMinuteCount = await unitOfWork.GetEntityRepository<ContactVerification>().DbSet
            .Where(x => x.UserId == request.UserId
                        && x.Type == ContactVerificationType.Phone
                        && x.CreatedDate >= oneMinuteAgo)
            .CountAsync(cancellationToken);

        if (lastMinuteCount >= 1)
            return Result.Fail<Unit>(ErrorsCodes.PhoneVerificationRateLimited);
        
        var code = GenerateCode(6);

        var entity = new ContactVerification
        {
            UserId = request.UserId!.Value,
            Type = ContactVerificationType.Phone,
            Destination = request.PhoneE164,
            Code = code,
            ExpiresAt = timeProvider.GetUtcNow().UtcDateTime.AddMinutes(QatarResidentOtpConstants.OtpExpiryMinutes)
        };

        await unitOfWork.GetEntityRepository<ContactVerification>().AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var model = new PhoneVerificationCodeModel
        {
            Code = code,
            ExpiryMinutes = QatarResidentOtpConstants.OtpExpiryMinutes
        };

        var notification = Notification.Create(NotificationChannel.Sms, 
            PhoneVerificationCodeModel.TemplateKey, 
            request.UserId.Value, request.PhoneE164, 
            null, 
            null, 
            null, JsonSerializer.Serialize(model), 
            user.GetPreferredLanguage(), null, 0);
        await unitOfWork.GetEntityRepository<Notification>().AddAsync(notification, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }

    private static string GenerateCode(int length)
    {
        const string digits = "0123456789";
        var bytes = RandomNumberGenerator.GetBytes(length);
        var chars = new char[length];
        for (int i = 0; i < length; i++)
        {
            chars[i] = digits[bytes[i] % digits.Length];
        }
        return new string(chars);
    }
}

