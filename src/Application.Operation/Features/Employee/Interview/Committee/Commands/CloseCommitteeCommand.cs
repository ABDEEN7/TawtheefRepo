using System;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Committee.Commands;

public sealed record CloseCommitteeCommand(Guid Id) : IRequest<IResult<Unit>>;
