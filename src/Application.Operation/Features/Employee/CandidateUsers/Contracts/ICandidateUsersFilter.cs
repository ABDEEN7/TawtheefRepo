using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.CandidateUsers.Contracts;

public interface ICandidateUsersFilter
{
    string? Name { get; }
    string? Email { get; }
    string? Qid { get; }
    string? MobileNumber { get; }
    UserProfileStatus? ProfileStatus { get; }
    int? Year { get; }
    CandidateUsersResultScope Scope { get; }
}
