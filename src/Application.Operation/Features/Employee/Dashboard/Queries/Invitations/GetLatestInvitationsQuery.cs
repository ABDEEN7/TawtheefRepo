using Application.Operation.Features.Employee.Dashboard.DTOs.Invitations;
using Application.Operation.Features.Employee.Dashboard.Queries.Common;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Dashboard.Queries.Invitations;

public sealed record GetLatestInvitationsQuery : DashboardQueryBase, IRequest<Result<IReadOnlyList<LatestInvitationDto>>>;
