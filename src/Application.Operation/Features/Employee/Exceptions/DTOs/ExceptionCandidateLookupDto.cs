using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Exceptions.DTOs;

public sealed record ExceptionCandidateLookupDto(
    Guid ApplicantId,
    Guid ProfileId,
    string Qid,
    string CandidateName,
    UserProfileStatus ProfileStatus);
