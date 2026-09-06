using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Locations.Commands;

public sealed record DeleteLocationCommand(Guid LocationId) : IRequest<IResult<Unit>>;
