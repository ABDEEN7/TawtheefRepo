using Cortex.Mediator.Queries;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Admin.Users.Queries;

public sealed record GetUserAssignedRoleIdsQuery(Guid UserId) : IQuery<IResult<IReadOnlyCollection<Guid>>>;
