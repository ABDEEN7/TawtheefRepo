using Application.Operation.Features.Admin.Universities.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Universities.Queries;

public sealed record GetUniversityDetailsQuery(Guid Id) : IRequest<IResult<UniversityAdminDto>>;

