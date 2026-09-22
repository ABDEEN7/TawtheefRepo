using Application.Operation.Features.Employee.TestSlots.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.TestSlots.Queries;

public sealed record GetTestSlotSessionsQuery(Guid TestSlotId, string Language) : IRequest<IResult<List<TestSlotSessionDto>>>;
