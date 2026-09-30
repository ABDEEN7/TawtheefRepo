using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.ResultReport.Services;

public sealed record SuggestionCandidate(Guid CandidateId, bool IsQualified, bool IsQatari, decimal FinalScore);

// Pure, dynamic calculation - never persisted (scheduleResult.md: "the system suggestion is not
// itself a persisted business state"). Ranks the whole report against the job's open vacancies:
//  - not qualified -> Rejected;
//  - qualified candidates fill the open vacancies (NumberOfVacancies minus candidates already decided
//    CandidateForHiringProcess in the job's other reports) Qatari first by FinalScore desc, then the
//    others by FinalScore desc -> CandidateForHiringProcess;
//  - qualified candidates left over once the vacancies are filled -> WaitingList (Qatari included).
// Equal scores fall back to the candidate id so the suggestion is stable between reads.
public static class ResultCandidateSuggestionService
{
    public static IReadOnlyDictionary<Guid, FinalDecision> Suggest(
        IEnumerable<SuggestionCandidate> candidates, int numberOfVacancies, int alreadyHiringCountForJob)
    {
        var openVacancies = Math.Max(0, numberOfVacancies - alreadyHiringCountForJob);
        var list = candidates.ToList();

        var result = list
            .Where(c => !c.IsQualified)
            .ToDictionary(c => c.CandidateId, _ => FinalDecision.Rejected);

        var ranked = list
            .Where(c => c.IsQualified)
            .OrderByDescending(c => c.IsQatari)
            .ThenByDescending(c => c.FinalScore)
            .ThenBy(c => c.CandidateId);

        foreach (var candidate in ranked)
        {
            result[candidate.CandidateId] = openVacancies > 0
                ? FinalDecision.CandidateForHiringProcess
                : FinalDecision.WaitingList;
            if (openVacancies > 0)
                openVacancies--;
        }

        return result;
    }
}
