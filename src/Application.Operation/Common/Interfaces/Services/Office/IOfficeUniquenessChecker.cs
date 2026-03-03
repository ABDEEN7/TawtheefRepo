namespace Application.Operation.Common.Interfaces.Services.Office;

public interface IOfficeUniquenessChecker
{
    Task<bool> OfficeAdminEmailExistsAsync(string email, CancellationToken ct);
    Task<bool> OfficeCountryExistsAsync(Guid countryId, CancellationToken ct);
}