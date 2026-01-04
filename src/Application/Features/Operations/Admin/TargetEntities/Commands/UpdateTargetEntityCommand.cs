using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Admin.TargetEntities.Commands;

public sealed record UpdateTargetEntityCommand(
    Guid Id,
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    bool IsActive) : IRequest<IResult<Guid>>;
