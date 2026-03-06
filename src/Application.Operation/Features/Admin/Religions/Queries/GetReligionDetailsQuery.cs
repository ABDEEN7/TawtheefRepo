using Application.Operation.Features.Admin.Religions.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Religions.Queries;

public sealed record GetReligionDetailsQuery(Guid Id) : IRequest<IResult<ReligionAdminDto>>;

