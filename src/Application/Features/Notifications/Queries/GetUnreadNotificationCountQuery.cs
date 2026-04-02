using MediatR;
using FluentResults;

namespace Tawtheef.Application.Features.Notifications.Queries;

public sealed record GetUnreadNotificationCountQuery(Guid UserId)
    : IRequest<IResult<int>>;
