using System;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Committee.Commands;

public sealed record SubmitCommitteeCommand(Guid Id) : IRequest<IResult<Unit>>;
