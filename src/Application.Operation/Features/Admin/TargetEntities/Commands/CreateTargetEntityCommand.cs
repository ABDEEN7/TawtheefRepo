using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.TargetEntities.Commands;

public sealed record CreateTargetEntityCommand(
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    bool IsActive) : IRequest<IResult<Guid>>;

