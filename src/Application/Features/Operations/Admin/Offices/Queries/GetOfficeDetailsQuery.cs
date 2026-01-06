using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Features.Operations.Admin.Offices.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Queries;

public sealed record GetOfficeDetailsQuery(Guid Id) : IQuery<IResult<OfficeDetailsDto>>;
