using System;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Commands;

public sealed record ApproveTemplateVersionCommand(Guid Id, string? DecisionNotes) : IRequest<IResult<Unit>>;
