using Application.Operation.Features.Admin.Offices.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Admin.Offices.Queries;

public sealed record GetOfficeDetailsQuery(Guid Id) : IQuery<IResult<OfficeDetailsDto>>;
