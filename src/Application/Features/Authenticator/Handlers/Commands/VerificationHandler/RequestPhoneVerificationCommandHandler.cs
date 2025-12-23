using System.Security.Cryptography;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands.VerificationHandler;


public class RequestPhoneVerificationCommandHandler(
    IUnitOfWork unitOfWork,
    ISmsSender smsSender,
    TimeProvider now,
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
        
        var oneMinuteAgo = now.GetUtcNow().AddMinutes(-1);
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
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10)
        };

        await unitOfWork.GetEntityRepository<ContactVerification>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var message = $"Your verification code is: {code}";
        _ = smsSender.SendAsync(request.PhoneE164, message, cancellationToken);

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
