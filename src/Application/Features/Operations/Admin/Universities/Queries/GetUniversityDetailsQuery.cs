using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Admin.Universities.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Universities.Queries;

public sealed record GetUniversityDetailsQuery(Guid Id) : IRequest<IResult<UniversityAdminDto>>;
