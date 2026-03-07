using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Offices.Commands;

public sealed record ChangeOfficeAdminCommand(Guid OfficeId, Guid UserId) : IRequest<IResult<Unit>>;

