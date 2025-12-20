using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Commands;

public sealed record SetOfficeAdminCommand(Guid OfficeId, Guid UserId) : IRequest<IResult<Unit>>;
