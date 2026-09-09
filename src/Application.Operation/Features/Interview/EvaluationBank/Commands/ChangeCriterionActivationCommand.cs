using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Application.Operation.Features.Interview.EvaluationBank.Commands;

public sealed record ChangeCriterionActivationCommand(Guid Id, bool IsActive) : IRequest<IResult<Unit>>;
