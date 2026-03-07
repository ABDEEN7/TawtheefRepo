using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Religions.Commands;

public sealed record CreateReligionCommand(
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    bool IsActive) : IRequest<IResult<Guid>>;

