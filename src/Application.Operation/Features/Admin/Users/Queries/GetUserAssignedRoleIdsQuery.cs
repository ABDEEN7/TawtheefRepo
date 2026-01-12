using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Admin.Users.Queries;

public sealed record GetUserAssignedRoleIdsQuery(Guid UserId) : IQuery<IResult<IReadOnlyCollection<Guid>>>;
