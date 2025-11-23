using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Recruitment.Dashboard.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Dashboard.Queries;

public sealed record GetCandidateInvitationsQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<CandidateInvitationsDto>>>
{
    public Guid? InvitationStatusId { get; set; }
    public Guid? JobCategoryId { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? JobTitle { get; set; }
}
