using System;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Committee.Commands;

public sealed record ReturnCommitteeCommand(Guid Id, string Reason) : IRequest<IResult<Unit>>;
