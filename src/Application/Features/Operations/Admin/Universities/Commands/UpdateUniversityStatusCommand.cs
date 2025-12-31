using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Admin.Universities.Commands;

public sealed record UpdateUniversityStatusCommand(Guid UniversityId, bool IsActive) : IRequest<IResult<Unit>>;
