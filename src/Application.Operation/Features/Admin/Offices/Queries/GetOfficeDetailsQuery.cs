using Application.Operation.Features.Admin.Offices.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Offices.Queries;

public sealed record GetOfficeDetailsQuery(Guid Id) : IRequest<IResult<OfficeDetailsDto>>;

