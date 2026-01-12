using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Utilities;

internal static class TrainingPointsCalculator
{
    private const string TrainingHighCode = "highLinked";
    private const string TrainingMediumCode = "mediumLinked";
    private const string TrainingLowCode = "lowLinked";

    public static int Calculate(ICollection<TrainingCourse>? trainingCourses, JobPointsMain jobPoints)
    {
        var details = jobPoints.Details
            .Where(d => d is { Type: JobPointRuleType.Training, IsDeleted: false })
            .ToList();

        if (details.Count == 0 || trainingCourses == null)
            return 0;

        var points = trainingCourses.Select(training => training.SpecializationRelation switch
            {
                //TODO : Must be checked later
                SpecializationRelationLevel.Strong => TrainingHighCode,
                SpecializationRelationLevel.Medium => TrainingMediumCode,
                SpecializationRelationLevel.Weak => TrainingLowCode,
                _ => null
            })
            .OfType<string>()
            .Sum(detailCode => JobPointsHelpers.GetDetailPoints(details, detailCode));

        return JobPointsHelpers.Clamp(points, jobPoints.Training);
    }
}
