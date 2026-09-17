using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.ResultReport.Services;

// Pure, dynamic calculation - never persisted (scheduleResult.md: "the system suggestion is not
// itself a persisted business state"). Confirmed formula: not qualified -> Rejected; qualified +
// Qatari -> CandidateForHiringProcess unconditionally; qualified + not Qatari -> depends on whether
// this job still has an open vacancy slot (counting candidates already decided CandidateForHiringProcess
// for the same job) -> CandidateForHiringProcess or WaitingList.
public static class ResultCandidateSuggestionService
{
    public static FinalDecision Suggest(bool isQualified, bool isQatari, int numberOfVacancies, int alreadyHiringCountForJob)
    {
        if (!isQualified)
            return FinalDecision.Rejected;

        if (isQatari)
            return FinalDecision.CandidateForHiringProcess;

        return alreadyHiringCountForJob < numberOfVacancies
            ? FinalDecision.CandidateForHiringProcess
            : FinalDecision.WaitingList;
    }
}
