using System;
using System.Collections.Generic;
using System.Text;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.EvaluationBank.Commands;

public sealed record CreateCriterionCommand(
    Guid InterviewEvaluationAxisId,
     string NameAr,
    string? NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    bool IsActive) : IRequest<Result<Guid>>;
