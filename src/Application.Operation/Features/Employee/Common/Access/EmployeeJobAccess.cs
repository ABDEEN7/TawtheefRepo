using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Common.Access;

public sealed record EmployeeJobAccessContext(Guid? CurrentUserId, bool HasFullAccess);

public sealed class EmployeeJobAccessContextProvider(
    ICurrentUserService currentUserService,
    IHttpContextAccessor httpContextAccessor)
{
    public EmployeeJobAccessContext GetAccess()
    {
        var currentUserId = Guid.TryParse(currentUserService.UserId, out var parsedUserId)
            ? parsedUserId
            : (Guid?)null;
        var hasFullAccess = httpContextAccessor.HttpContext?.User.HasFullJobAccess() ?? false;
        return new EmployeeJobAccessContext(currentUserId, hasFullAccess);
    }
}

public static class EmployeeJobAccessScope
{
    public static IQueryable<Job> ApplyJobAccessScope(
        this IQueryable<Job> jobs,
        EmployeeJobAccessContext context)
    {
        if (context.HasFullAccess) return jobs;

        return context.CurrentUserId.HasValue
            ? jobs.Where(job => job.CreatedById == context.CurrentUserId.Value)
            : jobs.Where(_ => false);
    }
}
