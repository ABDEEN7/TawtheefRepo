using Application.Operation.Features.Admin.TargetEntities.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Admin.TargetEntities.Queries;

public sealed record GetTargetEntityDetailsQuery(Guid Id) : IQuery<IResult<TargetEntityAdminDto>>;
