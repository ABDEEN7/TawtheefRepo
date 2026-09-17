using System;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.Commands;

public sealed record UpdateTemplateVersionCriterionCommand(
    Guid Id,
    Guid? InterviewEvaluationCriterionId,
    string? NameAr,
    string? NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    decimal MaxScore,
    bool IsRequired,
    int OrderNo,
    string? Notes) : IRequest<IResult<Unit>>;
