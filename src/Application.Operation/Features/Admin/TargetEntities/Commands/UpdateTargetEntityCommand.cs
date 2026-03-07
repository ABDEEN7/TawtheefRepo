using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.TargetEntities.Commands;

public sealed record UpdateTargetEntityCommand(
    Guid Id,
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    bool IsActive) : IRequest<IResult<Guid>>;

