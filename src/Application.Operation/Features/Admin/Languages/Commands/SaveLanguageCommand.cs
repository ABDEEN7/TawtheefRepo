using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Languages.Commands;

public sealed record SaveLanguageCommand(
    Guid? Id,
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    bool IsActive) : IRequest<IResult<Guid>>;

