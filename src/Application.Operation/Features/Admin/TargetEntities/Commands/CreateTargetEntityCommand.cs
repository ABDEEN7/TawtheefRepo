using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Admin.TargetEntities.Commands;

public sealed record CreateTargetEntityCommand(
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    bool IsActive) : ICommand<IResult<Guid>>;
