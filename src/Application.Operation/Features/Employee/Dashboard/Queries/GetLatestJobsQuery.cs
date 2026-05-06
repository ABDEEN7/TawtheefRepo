using Application.Operation.Features.Employee.Dashboard.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Dashboard.Queries;

public sealed record GetLatestJobsQuery : DashboardQueryBase, IRequest<Result<IReadOnlyList<LatestJobDto>>>;
