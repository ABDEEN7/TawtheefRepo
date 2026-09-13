using System;
using Application.Operation.Features.Interview.EvaluationTemplate.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Queries;

public sealed record GetTemplateVersionDetailsQuery(Guid Id) : IRequest<IResult<TemplateVersionDetailsDto>>;
