using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Offices.Commands;

public sealed record DeleteOfficeCommand(Guid UserId,Guid OfficeId) : IRequest<IResult<Unit>>;

