using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Commands;

public sealed record ChangeOfficeAdminCommand(Guid OfficeId, Guid UserId) : IRequest<IResult<Unit>>;
