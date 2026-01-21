using Application.Operation.Common.Interfaces.Services.Office;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Services.Office;


public sealed class OfficeUniquenessChecker(UserManager<User> userManager)
    : IOfficeUniquenessChecker
{
    public async Task<bool> OfficeAdminEmailExistsAsync(string email, CancellationToken ct)
    {
        var normalized = email.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            return false;

        return await userManager.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => u.Email == normalized, ct);
    }
}
