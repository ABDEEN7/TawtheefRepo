using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Admin.Religions.Commands;

public sealed record UpdateReligionCommand(
    Guid Id,
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    bool IsActive) : ICommand<IResult<Guid>>;
