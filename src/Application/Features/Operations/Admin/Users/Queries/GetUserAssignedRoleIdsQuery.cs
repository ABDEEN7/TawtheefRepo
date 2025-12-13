using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Admin.Users.Queries;

public sealed record GetUserAssignedRoleIdsQuery(Guid UserId) : IRequest<IResult<IReadOnlyCollection<Guid>>>;
