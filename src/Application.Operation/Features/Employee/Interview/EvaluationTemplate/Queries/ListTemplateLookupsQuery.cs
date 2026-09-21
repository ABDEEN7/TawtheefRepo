using System;
using System.Collections.Generic;
using System.Text;
using Application.Operation.Features.Employee.Interview.EvaluationTemplate.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.Queries;

public sealed record ListTemplateLookupsQuery() : IRequest<IResult<TemplateLookupsDto>>;
