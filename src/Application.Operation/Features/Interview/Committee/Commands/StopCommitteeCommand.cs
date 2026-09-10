using System;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Interview.Committee.Commands;

public sealed record StopCommitteeCommand(Guid Id, string? Reason) : IRequest<IResult<Unit>>;
