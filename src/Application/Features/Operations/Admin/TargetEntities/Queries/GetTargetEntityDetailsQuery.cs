using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Admin.TargetEntities.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.TargetEntities.Queries;

public sealed record GetTargetEntityDetailsQuery(Guid Id) : IRequest<IResult<TargetEntityAdminDto>>;
