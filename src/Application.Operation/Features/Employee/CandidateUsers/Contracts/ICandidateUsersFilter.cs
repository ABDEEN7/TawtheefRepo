using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.CandidateUsers.Contracts;

public interface ICandidateUsersFilter
{
    string? Search { get; }

    bool? IsBlocked { get; }

    List<UserProfileStatus>? ProfileStatuses { get; }

    UserProfileStatus? ProfileStatus { get; }

    int? Year { get; }

    CandidateUsersResultScope Scope { get; }
}
