using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Lookups.Queries;

public sealed record GetCandidateTypesByProviderQuery(string Provider)
    : BaseSearchQuery, IRequest<IResult<List<DropdownOptions>>>;
