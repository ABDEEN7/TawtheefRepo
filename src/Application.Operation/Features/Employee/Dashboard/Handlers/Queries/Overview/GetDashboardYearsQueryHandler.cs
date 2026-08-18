using Application.Operation.Features.Employee.Dashboard.Queries.Overview;
using Application.Operation.Features.Employee.Dashboard.Services.Access;
using Application.Operation.Features.Employee.Dashboard.Services.Scopes;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Operation.Features.Employee.Dashboard.Handlers.Queries.Overview;

internal sealed class GetDashboardYearsQueryHandler(
    DashboardAccessContextProvider accessContextProvider,
    DashboardQueryScope scope)
    : IRequestHandler<GetDashboardYearsQuery, Result<IReadOnlyList<int>>>
{
    public async Task<Result<IReadOnlyList<int>>> Handle(
        GetDashboardYearsQuery request,
        CancellationToken cancellationToken)
    {
        var contextResult = await accessContextProvider.GetAsync(cancellationToken);
        if (contextResult.IsFailed) return Result.Fail(contextResult.Errors);

        var context = contextResult.Value;
        var profileYears = await scope.AccessibleProfiles(context)
            .Select(profile => profile.CreatedDate.Year)
            .Distinct()
            .ToListAsync(cancellationToken);
        var jobYears = await scope.AccessibleJobs(context)
            .Select(job => job.CreatedDate.Year)
            .Distinct()
            .ToListAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<int>>(profileYears
            .Concat(jobYears)
            .Distinct()
            .OrderByDescending(year => year)
            .ToList());
    }
}
