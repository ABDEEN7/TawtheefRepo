using Application.Recruitment.Features.Dashboard.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Recruitment.Features.Dashboard.Queries;

public sealed record GetCandidateInvitationsQuery(Guid? UserId, Guid? InvitationStatusId,Guid? JobCategoryId,Guid? DepartmentId,string? JobTitle)
    : PaginatedRequest, IRequest<IResult<PaginatedResult<CandidateInvitationsDto>>>;

