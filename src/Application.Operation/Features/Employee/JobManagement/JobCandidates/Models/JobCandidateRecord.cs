using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;

public record JobCandidateRecord
{
    public Guid? InvitationId { get; init; }
    public Guid ApplicantId { get; init; }
    public User? Applicant { get; init; }
    public UserProfile? Profile { get; init; }

    public Guid JobId { get; init; }
    public Tawtheef.Domain.Entities.Recruitment.Job? Job { get; init; }
    public Major? Major { get; init; }

    public Guid? InvitationStatusId { get; init; } // or int? depending on your model
    public int Points { get; init; }
    public DateTimeOffset? CreatedDate { get; init; }
}
