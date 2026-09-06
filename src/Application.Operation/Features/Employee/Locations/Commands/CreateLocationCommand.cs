using Application.Operation.Features.Employee.Locations.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Locations.Commands;

public sealed record CreateLocationCommand(SaveLocationDto Location) : IRequest<IResult<Guid>>;
