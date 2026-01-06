using Cortex.Mediator.Queries;
using FluentResults;

using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Recruitment.Dashboard.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Dashboard.Queries;

public sealed record GetCandidateInvitationsQuery(
    Guid? InvitationStatusId,
    Guid? JobCategoryId,
    Guid? DepartmentId,
    string? JobTitle)
    : PaginatedRequest, IQuery<IResult<PaginatedResult<CandidateInvitationsDto>>>;
