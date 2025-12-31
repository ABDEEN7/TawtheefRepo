using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Admin.Languages.Commands;

public sealed record SaveLanguageCommand(
    Guid? Id,
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    bool IsActive) : IRequest<IResult<Guid>>;
