using Tawtheef.Domain.Entities.Exams;

namespace Application.Recruitment.Features.CandidateExams.Models;

internal sealed record StartCandidateExamPartData(
    TestAttemptPart Part, int PartNo, int DurationMinutes);
