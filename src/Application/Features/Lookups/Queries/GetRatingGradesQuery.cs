using CSharpFunctionalExtensions;
using MediatR;
using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Lookups.Queries;

public sealed record GetRatingGradesQuery : IRequest<Result<List<DropdownOptions>>>;
