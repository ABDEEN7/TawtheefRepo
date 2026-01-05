using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Features.Operations.Admin.Religions.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Religions.Queries;

public sealed record GetReligionDetailsQuery(Guid Id) : IQuery<IResult<ReligionAdminDto>>;
