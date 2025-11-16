using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Services.Identity;

public sealed class PasswordVerifier(IPasswordHasher<User> hasher) : IPasswordVerifier
{
    public bool Verify(User user, string plainPassword)
    {
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash!, plainPassword);
        return result == PasswordVerificationResult.Success;
    }
}