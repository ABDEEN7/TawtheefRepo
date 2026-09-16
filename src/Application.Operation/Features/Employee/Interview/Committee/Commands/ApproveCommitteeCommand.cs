using System;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Committee.Commands;

public sealed record ApproveCommitteeCommand(Guid Id, string? DecisionNotes) : IRequest<IResult<Unit>>;
