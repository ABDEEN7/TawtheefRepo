using Application.Operation.Features.Employee.Interview.EvaluationTemplate.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.Queries;

public sealed record ListTemplatesQuery() : IRequest<IResult<List<TemplateDto>>>;
