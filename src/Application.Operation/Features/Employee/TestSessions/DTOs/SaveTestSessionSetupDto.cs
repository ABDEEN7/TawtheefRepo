using Tawtheef.Domain.Entities.Exams;

namespace Application.Operation.Features.Employee.TestSessions.DTOs;

public sealed record SaveTestSessionSetupDto(
    Guid? TestSessionId,
    Guid ExamId,
    Guid? TestSlotId,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    TestSessionGenderFilter? GenderFilter,
    TestSessionNationalityFilter? NationalityFilter,
    IReadOnlyCollection<Guid>? InvitationIds,
    bool SendToApprove,
    string Language = "en");
