using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Features.Operations.Admin.TargetEntities.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.TargetEntities.Queries;

public sealed record GetTargetEntityDetailsQuery(Guid Id) : IQuery<IResult<TargetEntityAdminDto>>;
