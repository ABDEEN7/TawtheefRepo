using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using ILogger = Serilog.ILogger;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Utilities;

internal static class JobCandidatePointsCalculator
{
    public static int Calculate(
        JobCandidateRecord candidate,
        JobPointsMain jobPointsMain,
        List<JobDegree> jobDegrees,
        Guid jobMajorId,
        Guid? jobSubMajorId,
        ILogger logger
        )
    {
        var breakdown = CalculateBreakdown(
            candidate,
            jobPointsMain,
            jobDegrees,
            jobMajorId,
            jobSubMajorId,
            logger);

        return breakdown.TotalPoints;
    }

    public static JobCandidatePointsBreakdown CalculateBreakdown(
        JobCandidateRecord candidate,
        JobPointsMain jobPointsMain,
        List<JobDegree> jobDegrees,
        Guid jobMajorId,
        Guid? jobSubMajorId,
        ILogger logger)
    {
        if (candidate.Profile == null)
        {
            logger.Warning("Candidate {CandidateId} has no profile.", candidate.ApplicantId);
            return new JobCandidatePointsBreakdown(0, 0, 0, 0, 0, 0, 0);
        }

        var profile = candidate.Profile;

        var categoryPoints =
            ApplicantCategoryPointsCalculator.Calculate(profile.CandidateType, jobPointsMain);

        var educationPoints =
            EducationPointsCalculator.Calculate(
                profile.Qualifications,
                jobPointsMain,
                jobDegrees,
                jobMajorId,
                jobSubMajorId);

        var experiencePoints =
            ExperiencePointsCalculator.Calculate(profile.CalculatedExperienceYears, jobPointsMain);

        var trainingPoints =
            TrainingPointsCalculator.Calculate(profile.TrainingCourses, jobPointsMain);

        var skillPoints =
            SkillPointsCalculator.Calculate(profile.Skills, jobPointsMain);

        var languagePoints =
            LanguagePointsCalculator.Calculate(profile.Languages, jobPointsMain);

        var certificatePoints =
            CertificatesPointsCalculator.Calculate(profile.Achievements, jobPointsMain);

        logger.Information(
            """
            Points breakdown for Candidate {CandidateId}:
            - Category: {CategoryPoints}
            - Education: {EducationPoints}
            - Experience: {ExperiencePoints}
            - Training: {TrainingPoints}
            - Skills: {SkillPoints}
            - Languages: {LanguagePoints}
            - Certificates: {CertificatePoints}
            """,
            candidate.ApplicantId,
            categoryPoints,
            educationPoints,
            experiencePoints,
            trainingPoints,
            skillPoints,
            languagePoints,
            certificatePoints);

        return new JobCandidatePointsBreakdown(
            categoryPoints,
            educationPoints,
            experiencePoints,
            trainingPoints,
            skillPoints,
            languagePoints,
            certificatePoints);
    }
}
