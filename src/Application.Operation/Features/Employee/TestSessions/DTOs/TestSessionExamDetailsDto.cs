namespace Application.Operation.Features.Employee.TestSessions.DTOs;

public sealed record TestSessionExamDetailsDto(
    Guid ExamId,
    string ExamName,
    string JobTitle,
    string? Specialization,
    string? MainSpecialization,
    string? SubSpecialization,
    string ExamStatus,
    int TotalCandidates,
    int DurationMinutes,
    IReadOnlyList<TestSessionExamPartDto> Parts,
    int NumberOfQuestions,
    decimal? QualificationScore);

public sealed record TestSessionExamPartDto(int PartNo, int DurationMinutes);
