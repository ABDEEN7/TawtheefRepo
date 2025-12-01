using FluentResults;
using MediatR;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command;

public sealed record ReviewProfileItemCommand(
    Guid ReviewerId,
    Guid ReviewItemId,
    ReviewStatus Status,
    string? Note
) : IRequest<IResult<Unit>>;
