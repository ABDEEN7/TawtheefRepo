using Tawtheef.Domain.Entities.Exams;

namespace Application.Recruitment.Features.CandidateExams.Models;

internal sealed record CandidateExamExecutionState(
    Guid AttemptId,
    TestAttemptPart Part,
    TestAttemptQuestion Question,
    int TotalQuestions,
    bool AllowPreviousQuestion);
