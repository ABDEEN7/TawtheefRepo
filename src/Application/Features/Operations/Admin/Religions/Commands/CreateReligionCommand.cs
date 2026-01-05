using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Admin.Religions.Commands;

public sealed record CreateReligionCommand(
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    bool IsActive) : IRequest<IResult<Guid>>;
