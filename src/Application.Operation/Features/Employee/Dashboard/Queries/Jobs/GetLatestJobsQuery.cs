using Application.Operation.Features.Employee.Dashboard.DTOs.Jobs;
using Application.Operation.Features.Employee.Dashboard.Queries.Common;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Dashboard.Queries.Jobs;

public sealed record GetLatestJobsQuery : DashboardQueryBase, IRequest<Result<IReadOnlyList<LatestJobDto>>>;
