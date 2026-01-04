using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Admin.Religions.Commands;

public sealed record UpdateReligionStatusCommand(Guid ReligionId, bool IsActive) : IRequest<IResult<Unit>>;
