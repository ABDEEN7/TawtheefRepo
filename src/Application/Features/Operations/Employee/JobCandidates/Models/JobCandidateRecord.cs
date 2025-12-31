using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Models;

public sealed record JobCandidateRecord
{
    public Guid InvitationId { get; init; }
    public Guid ApplicantId { get; init; }
    public ApplicantUser? Applicant { get; init; }
    public UserProfile? Profile { get; init; }
    public Domain.Entities.Recruitment.Job? Job { get; init; }
    public Major? Major { get; init; }
    public Guid InvitationStatusId { get; init; }
    public int Points { get; init; }
    public DateTimeOffset CreatedDate { get; init; }
}
