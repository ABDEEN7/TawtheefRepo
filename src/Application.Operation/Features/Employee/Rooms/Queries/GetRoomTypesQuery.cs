using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Lookups.Queries;

namespace Application.Operation.Features.Employee.Rooms.Queries;

public sealed record GetRoomTypesQuery : BaseSearchQuery, IRequest<IResult<List<DropdownOptions>>>;
