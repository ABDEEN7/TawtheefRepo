using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Admin.Religions.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Religions.Queries;

public sealed record GetReligionDetailsQuery(Guid Id) : IRequest<IResult<ReligionAdminDto>>;
