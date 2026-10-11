using Tawtheef.Domain.Entities.Exams;

namespace Application.Recruitment.Features.CandidateExams.Models;

internal sealed record CandidateExamPartTransitionData(
    TestAttemptPart Part,
    int PartNo,
    int DurationMinutes,
    decimal? QualificationScore);
