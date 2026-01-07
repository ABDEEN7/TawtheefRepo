using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Utilities;

internal static class CertificatesPointsCalculator
{
    private const string CertificatesLinkedCode = "certificatesLinked";
    private const string CertificatesNotLinkedCode = "certificatesNotLinked";
    private const string PrizeCode = "prize";

    public static int Calculate(ICollection<Achievement>? achievements, JobPointsMain jobPoints)
    {
        var details = jobPoints.Details
            .Where(d => d is { Type: JobPointRuleType.Certificate, IsDeleted: false })
            .ToList();

        if (details.Count == 0 || achievements == null)
            return 0;

        var points = 0;

        foreach (var achievement in achievements)
        {
            if (achievement.AchievementTypeId == AchievementTypeIds.Certificate)
            {
                points += achievement.RelatedToSpecialization == true
                    ? JobPointsHelpers.GetDetailPoints(details, CertificatesLinkedCode)
                    : JobPointsHelpers.GetDetailPoints(details, CertificatesNotLinkedCode);
            }
            else if (achievement.AchievementTypeId == AchievementTypeIds.Award)
            {
                points += JobPointsHelpers.GetDetailPoints(details, PrizeCode);
            }
        }

        return JobPointsHelpers.Clamp(points, jobPoints.Certificates);
    }
}
