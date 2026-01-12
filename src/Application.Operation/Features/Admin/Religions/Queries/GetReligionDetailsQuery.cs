using Application.Operation.Features.Admin.Religions.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Admin.Religions.Queries;

public sealed record GetReligionDetailsQuery(Guid Id) : IQuery<IResult<ReligionAdminDto>>;
