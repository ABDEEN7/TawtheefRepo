using CSharpFunctionalExtensions;
using MediatR;
using Tawtheef.Application.Features.Recruitment.Dashboard.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Dashboard.Queries;

public sealed record GetCandidateInvitationsQuery : IRequest<Result<List<CandidateInvitationsDto>>>
{
    public Guid? InvitationStatusId { get; set; }
    public Guid? JobCategoryId { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? JobTitle { get; set; }
}
