using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Models;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Services;

internal static class JobCandidatesFilterProcessor
{
    public static List<JobCandidateRecord> ApplyPercentageFilters(
        IReadOnlyCollection<JobCandidateRecord> candidates,
        JobCandidateFilterSetting? settings,
        int targetCount) // NEW
    {
        if (candidates.Count == 0) return [];

        var typePercentages = settings?.CandidateTypePercentages
            .Where(item => item.Percentage > 0)
            .ToList();

        // No quota => just take top targetCount
        if (typePercentages is not { Count: > 0 })
            return candidates
                .OrderByDescending(c => c.Points)
                .Take(targetCount)
                .ToList();

        var filtered = new List<JobCandidateRecord>();

        // candidates are assumed already sorted by points DESC
        foreach (var typePercentage in typePercentages)
        {
            if (filtered.Count >= targetCount) break;

            var byType = candidates
                .Where(c => c.Profile?.CandidateTypeId == typePercentage.CandidateTypeId)
                .OrderByDescending(c => c.Points)
                .ToList();

            if (byType.Count == 0) continue;

            var typeTarget = CalculateTargetCount(targetCount, typePercentage.Percentage);
            typeTarget = Math.Min(typeTarget, byType.Count);

            if (typeTarget == 0) continue;

            if (IsNationalityBreakdownType(typePercentage.CandidateTypeId))
            {
                var natPercentages = settings?.NationalityPercentages
                    .Where(n => n.CandidateTypeId == typePercentage.CandidateTypeId && n.Percentage > 0)
                    .ToList();

                if (natPercentages is { Count: > 0 })
                {
                    filtered.AddRange(ApplyNationalityBreakdown(byType, natPercentages, typeTarget));
                    continue;
                }
            }

            filtered.AddRange(byType.Take(typeTarget));
        }

        filtered = filtered
            .DistinctBy(c => c.ApplicantId)
            .ToList();

        // BRD: fill remainder from best remaining
        if (filtered.Count < targetCount)
        {
            var remaining = candidates
                .Where(c => filtered.All(x => x.ApplicantId != c.ApplicantId))
                .OrderByDescending(c => c.Points);

            filtered.AddRange(remaining.Take(targetCount - filtered.Count));
        }

        return filtered
            .DistinctBy(c => c.ApplicantId)
            .Take(targetCount)
            .ToList();
    }

    public static IEnumerable<JobCandidateRecord> ApplySorting(
        IEnumerable<JobCandidateRecord> candidates,
        string? sortBy,
        string? sortDirection)
    {
        var descending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        return sortBy?.ToLowerInvariant() switch
        {
            "points" => descending
                ? candidates.OrderByDescending(candidate => candidate.Points)
                : candidates.OrderBy(candidate => candidate.Points),
            "createddate" or null or "" => descending
                ? candidates.OrderByDescending(candidate => candidate.CreatedDate)
                : candidates.OrderBy(candidate => candidate.CreatedDate),
            _ => descending
                ? candidates.OrderByDescending(candidate => candidate.CreatedDate)
                : candidates.OrderBy(candidate => candidate.CreatedDate)
        };
    }

    private static bool IsNationalityBreakdownType(Guid candidateTypeId)
        => candidateTypeId == CandidateTypeIds.ResidentQatar || candidateTypeId == CandidateTypeIds.NonQatari;

    private static List<JobCandidateRecord> ApplyNationalityBreakdown(
        IEnumerable<JobCandidateRecord> candidates,
        IReadOnlyCollection<JobCandidateNationalityPercentage> nationalityPercentages,
        int targetCount)
    {
        var filteredCandidates = new List<JobCandidateRecord>();

        foreach (var nationalityPercentage in nationalityPercentages)
        {
            var candidatesByNationality = candidates
                .Where(candidate => candidate.Profile?.NationalityId == nationalityPercentage.NationalityId)
                .OrderByDescending(candidate => candidate.Points)
                .ToList();

            if (candidatesByNationality.Count == 0)
            {
                continue;
            }

            var nationalityTarget = CalculateTargetCount(targetCount, nationalityPercentage.Percentage);
            nationalityTarget = Math.Min(nationalityTarget, candidatesByNationality.Count);

            if (nationalityTarget == 0)
            {
                continue;
            }

            filteredCandidates.AddRange(candidatesByNationality.Take(nationalityTarget));
        }

        return filteredCandidates;
    }

    private static int CalculateTargetCount(int total, int percentage)
        => total <= 0 || percentage <= 0
            ? 0
            : (int)Math.Round(total * (percentage / 100.0), MidpointRounding.AwayFromZero);
}
