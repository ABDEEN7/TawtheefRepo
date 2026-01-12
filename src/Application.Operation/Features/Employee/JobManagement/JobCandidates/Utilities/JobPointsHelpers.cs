using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Features.Employee.JobCandidates.Utilities;

internal static class JobPointsHelpers
{
    public static int GetDetailPoints(IEnumerable<JobPointsDetail> details, string code) =>
        details
            .Where(d => string.Equals(d.Code, code, StringComparison.OrdinalIgnoreCase))
            .Select(d => d.Points)
            .FirstOrDefault();

    public static int Clamp(int value, int max) => max <= 0 ? 0 : (value > max ? max : value);
}
