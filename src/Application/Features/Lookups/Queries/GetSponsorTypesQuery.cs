using CSharpFunctionalExtensions;
using MediatR;
using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Lookups.Queries;

public sealed record GetSponsorTypesQuery : IRequest<Result<List<DropdownOptions>>>;
