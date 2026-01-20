using System.Security.Cryptography;
using Application.Recruitment.Features.Authenticator.Commands.Verification;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Authenticator.Handlers.Commands.VerificationHandler;

public class RequestEmailVerificationCommandHandler(
    IUnitOfWork unitOfWork,
    UserManager<User> userManager)
    : ICommandHandler<RequestEmailVerificationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(RequestEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null)
            return Result.Fail<Unit>(ErrorsCodes.UserNotFound);
        
        var emailAlreadyUsed = await userManager.Users
            .AnyAsync(u => u.Email == request.Email && u.Id != request.UserId, cancellationToken);
        if (emailAlreadyUsed)
            return Result.Fail<Unit>(ErrorsCodes.EmailAlreadyInUse);
        
        
        var code = GenerateCode(6);
        var entity = new ContactVerification
        {
            UserId = request.UserId!.Value,
            Type = ContactVerificationType.Email,
            Destination = request.Email,
            Code = code,
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(30)
        };

        entity.Send();
        await unitOfWork.GetEntityRepository<ContactVerification>().AddAsync(entity);
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
