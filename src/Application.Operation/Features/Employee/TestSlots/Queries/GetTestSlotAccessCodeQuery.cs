using Application.Operation.Features.Employee.TestSlots.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.TestSlots.Queries;

public sealed record GetTestSlotAccessCodeQuery(Guid TestSlotId) : IRequest<IResult<TestSlotAccessCodeDto>>;
