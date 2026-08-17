using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Dashboard.Queries.Overview;

public sealed record GetDashboardYearsQuery : IRequest<Result<IReadOnlyList<int>>>;
