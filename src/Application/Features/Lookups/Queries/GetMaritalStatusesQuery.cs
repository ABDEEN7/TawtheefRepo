using MediatR;
using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Lookups.Queries;

public sealed record GetMaritalStatusesQuery : IRequest<List<DropdownOptions>>;