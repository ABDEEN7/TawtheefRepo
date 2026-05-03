using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models;

namespace Application.Operation.Features.Admin.Users.Queries;

public sealed record GetUsersLookupQuery : IRequest<IResult<List<DropdownOptions>>>;
