using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Application.Operation.Features.Interview.EvaluationBank.Commands;

public sealed record UpdateCriterionCommand(
    Guid Id,
    Guid InterviewEvaluationAxisId,
    string NameAr,
    string? NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    bool IsActive) : IRequest<IResult<Unit>>;
