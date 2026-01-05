using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Commands;

public sealed record CreateSectorCommand(
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    bool IsActive,
    int DisplayOrder) : IRequest<IResult<Guid>>;
