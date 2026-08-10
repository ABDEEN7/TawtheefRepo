using Application.Operation.Features.Employee.Dashboard.DTOs.Overview;
using Application.Operation.Features.Employee.Dashboard.Queries.Common;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Dashboard.Queries.Overview;

public sealed record GetDashboardOverviewQuery : DashboardQueryBase, IRequest<Result<DashboardOverviewDto>>;
