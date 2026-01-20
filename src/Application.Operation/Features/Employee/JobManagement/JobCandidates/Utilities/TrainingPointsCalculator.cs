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
                // TODO: Implement automatic determination of SpecializationRelation level between training courses and job degrees.
                // Current implementation relies on manually set SpecializationRelation values from TrainingCourse entities.
                // Consider: 
                // - Creating a service to match training course topics/titles with job degree specializations
                // - Using a mapping table or algorithm to calculate relation strength
                // - Implementing ML/NLP-based similarity scoring between course content and job requirements
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
