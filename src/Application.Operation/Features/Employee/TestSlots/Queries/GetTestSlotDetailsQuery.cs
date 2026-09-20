using Application.Operation.Features.Employee.TestSlots.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.TestSlots.Queries;

public sealed record GetTestSlotDetailsQuery(Guid TestSlotId, string Language) : IRequest<IResult<TestSlotDetailsDto>>;
