using System;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Commands;

public sealed record CreateTemplateCommand(
    string TitleAr,
    string? TitleEn,
    Guid? OrganizationScopeId,
    Guid? JobTitleId,
    Guid? DepartmentId,
    bool IsActive) : IRequest<IResult<Guid>>;
