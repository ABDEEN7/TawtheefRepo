using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Countries.Commands;

public sealed record CreateCountryCommand : IRequest<IResult<Guid>>
{
    public string NameAr { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public int Code { get; init; }
    public string ISOCode { get; init; } = string.Empty;
    public string CodeAlpha { get; init; } = string.Empty;
    public bool IsActive { get; init; } = true;
}
