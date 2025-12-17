using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Commands;

public sealed record DeleteOfficeCommand(Guid Id) : IRequest<IResult<Unit>>;
