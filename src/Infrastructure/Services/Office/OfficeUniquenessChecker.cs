using Application.Operation.Common.Interfaces.Services.Office;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Services.Office;

public sealed class OfficeUniquenessChecker(UserManager<User> userManager, IUnitOfWork unitOfWork)
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

    public async Task<bool> OfficeCountryExistsAsync(Guid countryId, CancellationToken ct)
    {
        if (countryId == Guid.Empty)
            return false;

        return await unitOfWork.GetEntityRepository<Domain.Entities.Lookups.NoneSeeds.Office>().DbSet
            .AsNoTracking()
            .AnyAsync(o => o.CountryId == countryId, ct);
    }
}
