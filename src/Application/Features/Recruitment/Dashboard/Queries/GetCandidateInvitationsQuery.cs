using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Recruitment.Dashboard.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Dashboard.Queries;

public sealed record GetCandidateInvitationsQuery(
    Guid? InvitationStatusId,
    Guid? JobCategoryId,
    Guid? DepartmentId,
    string? JobTitle)
    : PaginatedRequest, IRequest<IResult<PaginatedResult<CandidateInvitationsDto>>>;
