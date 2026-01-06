using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Features.Operations.Admin.Universities.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Universities.Queries;

public sealed record GetUniversityDetailsQuery(Guid Id) : IQuery<IResult<UniversityAdminDto>>;
