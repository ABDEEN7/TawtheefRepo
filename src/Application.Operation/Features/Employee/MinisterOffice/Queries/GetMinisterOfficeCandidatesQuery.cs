using Application.Operation.Features.Employee.MinisterOffice.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.MinisterOffice.Queries;

public sealed record GetMinisterOfficeCandidatesQuery
    : PaginatedRequest,
        IRequest<IResult<PaginatedResult<MinisterOfficeCandidateDto>>>
{
    public string? SearchTerm { get; init; }
    public bool IncludeInactive { get; init; } = false;
    public Guid? GenderId { get; init; }
    public Guid? CandidateTypeId { get; init; }
    public Guid? TargetEntityId { get; init; }
}
