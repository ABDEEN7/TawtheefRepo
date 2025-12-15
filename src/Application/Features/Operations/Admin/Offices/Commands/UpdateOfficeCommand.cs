using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Admin.Offices.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Commands;

public sealed record UpdateOfficeCommand(
    Guid Id,
    string NameAr,
    string NameEn,
    IReadOnlyCollection<Guid> SupportedCountryIds) : IRequest<IResult<OfficeDto>>;
