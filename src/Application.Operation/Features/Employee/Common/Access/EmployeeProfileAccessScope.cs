using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Common.Access;

internal sealed class EmployeeProfileAccessScope(IUnitOfWork unitOfWork)
{
    public IQueryable<UserProfile> AccessibleProfiles(EmployeeProfileAccessContext context)
    {
        var profiles = ProfilePopulation(
            unitOfWork.GetEntityRepository<UserProfile>().DbSet.AsNoTracking(),
            context.CurrentUser);
        if (context.CanViewProfileDistribution) return profiles;
        if (!context.CanViewAssignedProfiles) return profiles.Where(_ => false);

        var assignments = unitOfWork.GetEntityRepository<ProfileAssignment>().DbSet;
        return profiles.Where(profile => assignments.Any(assignment =>
            assignment.UserProfileId == profile.Id &&
            assignment.EmployeeId == context.CurrentUserId &&
            assignment.IsActive &&
            assignment.UnassignedAtUtc == null));
    }

    internal static IQueryable<UserProfile> ProfilePopulation(
        IQueryable<UserProfile> profiles,
        User currentUser) =>
        currentUser switch
        {
            EmployeeUser => profiles.Where(profile =>
                profile.Provider == nameof(ProviderLoginIds.QatarPass) ||
                profile.Provider == nameof(ProviderLoginIds.QatarResidentOtp)),
            OfficeUser { Office.CountryId: var countryId } => profiles.Where(profile =>
                profile.ResidenceCountryId == countryId &&
                profile.Provider == nameof(ProviderLoginIds.Google)),
            _ => profiles.Where(_ => false)
        };
}
