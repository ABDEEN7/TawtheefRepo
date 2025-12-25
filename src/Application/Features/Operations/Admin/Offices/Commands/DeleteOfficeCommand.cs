using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Commands;

public sealed record DeleteOfficeCommand(Guid UserId,Guid OfficeId) : IRequest<IResult<Unit>>;
