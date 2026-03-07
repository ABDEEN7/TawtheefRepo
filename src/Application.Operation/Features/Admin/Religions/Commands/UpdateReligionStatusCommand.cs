using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Religions.Commands;

public sealed record UpdateReligionStatusCommand(Guid ReligionId, bool IsActive) : IRequest<IResult<Unit>>;

