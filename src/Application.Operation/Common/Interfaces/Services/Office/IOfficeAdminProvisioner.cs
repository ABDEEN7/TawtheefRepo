using FluentResults;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Common.Interfaces.Services.Office;

public interface IOfficeAdminProvisioner
{
    Task<Result<User>> CreateOfficeAdminAsync(
        string email,
        string adminNameAr,
        string adminNameEn,
        CancellationToken ct);

    Task<Result> AssignOfficeAsync(Guid userId, Guid officeId, CancellationToken ct);
}
