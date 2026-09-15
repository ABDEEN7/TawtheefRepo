using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models;

namespace Application.Operation.Features.Employee.TestSlots.Queries;

public sealed record GetAvailableRoomsForTestSlotQuery(string Language) : IRequest<IResult<List<DropdownOptions>>>;
