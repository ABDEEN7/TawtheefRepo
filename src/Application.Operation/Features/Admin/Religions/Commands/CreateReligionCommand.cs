using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Admin.Religions.Commands;

public sealed record CreateReligionCommand(
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    bool IsActive) : ICommand<IResult<Guid>>;
