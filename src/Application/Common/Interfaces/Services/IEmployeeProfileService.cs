using FluentResults;
using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Interfaces.Services;

public interface IEmployeeProfileService
{
    Task<IResult<EmployeeProfileInfo>> SyncFromDirectoryAsync(EmployeeUser user, CancellationToken ct = default);
}
