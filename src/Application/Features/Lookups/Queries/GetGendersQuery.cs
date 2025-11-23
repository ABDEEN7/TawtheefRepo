using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Lookups.Queries;

public sealed record GetGendersQuery : IRequest<IResult<List<DropdownOptions>>>;
