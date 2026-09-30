using Application.Operation.Features.Employee.Interview.OperationalIssue.DTOs;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.ResultReport.Services;

// Read-side pieces shared by the Approve Interview Results detail and the Committee Head Review, so
// both screens feed ResultCandidateSuggestionService the same inputs and show the same issues.
public static class ResultReportReadService
{
    // Scoped to the whole job (across every schedule/report for it), not just one report - a vacancy
    // is filled once, regardless of which interview round the candidate came through.
    public static Task<int> CountAlreadyHiringForJobAsync(IUnitOfWork unitOfWork, Guid jobId, CancellationToken cancellationToken) =>
        unitOfWork.GetEntityRepository<InterviewResultCandidate>().DbSet
            .Where(c => c.FinalDecision == FinalDecision.CandidateForHiringProcess
                && c.InterviewResultReport!.InterviewSchedule!.JobId == jobId)
            .CountAsync(cancellationToken);

    public static async Task<Dictionary<Guid, List<OperationalIssueDto>>> LoadIssuesByAppointmentAsync(
        IUnitOfWork unitOfWork, IReadOnlyCollection<Guid> appointmentIds, CancellationToken cancellationToken)
    {
        return (await unitOfWork.GetEntityRepository<InterviewOperationalIssue>().DbSet
            .AsNoTracking()
            .Where(i => appointmentIds.Contains(i.InterviewAppointmentId))
            .OrderByDescending(i => i.CreatedDate)
            .Select(i => new OperationalIssueDto(
                i.Id, i.InterviewAppointmentId, i.IssueType, i.Description, i.IsBlocking, i.Status,
                i.ResolvedById, i.ResolvedAt.AsUtcOffset(), i.ResolutionNotes, i.CreatedDate.AsUtcOffset()))
            .ToListAsync(cancellationToken))
            .GroupBy(i => i.InterviewAppointmentId)
            .ToDictionary(g => g.Key, g => g.ToList());
    }
}
