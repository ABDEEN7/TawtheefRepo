using Application.Operation.Features.Admin.TargetEntities.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.TargetEntities.Queries;

public sealed record GetTargetEntityDetailsQuery(Guid Id) : IRequest<IResult<TargetEntityAdminDto>>;

