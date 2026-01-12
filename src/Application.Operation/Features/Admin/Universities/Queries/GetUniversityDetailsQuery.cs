using Application.Operation.Features.Admin.Universities.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Admin.Universities.Queries;

public sealed record GetUniversityDetailsQuery(Guid Id) : IQuery<IResult<UniversityAdminDto>>;
