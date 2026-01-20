namespace Application.Operation.Common.Interfaces.Services.HttpClients;

public interface IOfficeUniquenessChecker
{
    Task<bool> OfficeAdminEmailExistsAsync(string email, CancellationToken ct);
}