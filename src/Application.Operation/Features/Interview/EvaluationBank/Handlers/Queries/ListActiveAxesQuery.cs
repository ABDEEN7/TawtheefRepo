using Application.Operation.Features.Interview.EvaluationBank.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Interview.EvaluationBank.Handlers.Queries;

public sealed record ListActiveAxesQuery() : IRequest<IResult<List<AxisDto>>>;
