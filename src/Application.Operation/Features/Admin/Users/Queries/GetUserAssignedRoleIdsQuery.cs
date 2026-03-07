using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Users.Queries;

public sealed record GetUserAssignedRoleIdsQuery(Guid UserId) : IRequest<IResult<IReadOnlyCollection<Guid>>>;

