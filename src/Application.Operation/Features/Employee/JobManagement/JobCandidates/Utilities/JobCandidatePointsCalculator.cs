using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Utilities;

internal static class JobCandidatePointsCalculator
{
    public static int Calculate(
        JobCandidateRecord candidate,
        JobPointsMain jobPointsMain,
        ICollection<JobDegree> jobDegrees,
        Guid? jobMajorId,
        Guid? jobSubMajorId,
        IAppLogger logger
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
        ICollection<JobDegree> jobDegrees,
        Guid? jobMajorId,
        Guid? jobSubMajorId,
        IAppLogger logger)
    {
        if (candidate.Profile == null)
        {
            logger.Warning("Candidate {CandidateId} has no profile.", candidate.ApplicantId);
            return new JobCandidatePointsBreakdown(0, 0, 0, 0, 0, 0, 0);
        }

        var profile = candidate.Profile;
        var details = new List<PointDetailDto>();

        var categoryPoints =
            ApplicantCategoryPointsCalculator.Calculate(profile.CandidateType, jobPointsMain);
        if (categoryPoints > 0)
            details.Add(new PointDetailDto
            {
                Section = "Category",
                SectionAr = "الفئة",
                SectionEn = "Category",
                Description = profile.CandidateType?.DescriptionEn ?? string.Empty,
                DescriptionAr = profile.CandidateType?.DescriptionAr ?? string.Empty,
                DescriptionEn = profile.CandidateType?.DescriptionEn ?? string.Empty,
                Points = categoryPoints
            });

        var educationPoints =
            EducationPointsCalculator.Calculate(
                profile.Qualifications,
                jobPointsMain,
                jobDegrees,
                jobMajorId,
                jobSubMajorId);
        if (educationPoints > 0)
        {
            var qual = profile.Qualifications?.OrderByDescending(q => q.CreatedDate).FirstOrDefault();
            details.Add(new PointDetailDto
            {
                Section = "Education",
                SectionAr = "التعليم",
                SectionEn = "Education",
                Description = qual?.Degree?.NameEn ?? "Qualification",
                DescriptionAr = qual?.Degree?.NameAr ?? "المؤهل العلمي",
                DescriptionEn = qual?.Degree?.NameEn ?? "Qualification",
                Points = educationPoints,
                ItemsAr = profile.Qualifications?.Select(q => q.Degree?.NameAr ?? string.Empty).ToList() ?? [],
                ItemsEn = profile.Qualifications?.Select(q => q.Degree?.NameEn ?? string.Empty).ToList() ?? []
            });
        }

        var experiencePoints =
            ExperiencePointsCalculator.Calculate(profile.CalculatedExperienceYears, jobPointsMain);
        if (experiencePoints > 0)
            details.Add(new PointDetailDto
            {
                Section = "Experience",
                SectionAr = "الخبرة",
                SectionEn = "Experience",
                Description = $"{profile.CalculatedExperienceYears} Years",
                DescriptionAr = $"{profile.CalculatedExperienceYears} سنوات",
                DescriptionEn = $"{profile.CalculatedExperienceYears} Years",
                Points = experiencePoints
            });

        var trainingPoints =
            TrainingPointsCalculator.Calculate(profile.TrainingCourses, jobPointsMain);
        if (trainingPoints > 0)
            details.Add(new PointDetailDto
            {
                Section = "Training",
                SectionAr = "التدريب",
                SectionEn = "Training",
                Description = $"{profile.TrainingCourses?.Count} Courses",
                DescriptionAr = $"{profile.TrainingCourses?.Count} دورات تدريبية",
                DescriptionEn = $"{profile.TrainingCourses?.Count} Courses",
                Points = trainingPoints,
                ItemsAr = profile.TrainingCourses?.Select(c => c.Title).ToList() ?? [],
                ItemsEn = profile.TrainingCourses?.Select(c => c.Title).ToList() ?? []
            });

        var skillPoints =
            SkillPointsCalculator.Calculate(profile.Skills, jobPointsMain);
        if (skillPoints > 0)
            details.Add(new PointDetailDto
            {
                Section = "Skills",
                SectionAr = "المهارات",
                SectionEn = "Skills",
                Description = $"{profile.Skills?.Count} Skills",
                DescriptionAr = $"{profile.Skills?.Count} مهارات",
                DescriptionEn = $"{profile.Skills?.Count} Skills",
                Points = skillPoints,
                ItemsAr = profile.Skills?.Select(s => s.Skill?.NameAr ?? string.Empty).ToList() ?? [],
                ItemsEn = profile.Skills?.Select(s => s.Skill?.NameEn ?? string.Empty).ToList() ?? []
            });

        var languagePoints =
            LanguagePointsCalculator.Calculate(profile.Languages, jobPointsMain);
        if (languagePoints > 0)
            details.Add(new PointDetailDto
            {
                Section = "Languages",
                SectionAr = "اللغات",
                SectionEn = "Languages",
                Description = $"{profile.Languages?.Count} Languages",
                DescriptionAr = $"{profile.Languages?.Count} لغات",
                DescriptionEn = $"{profile.Languages?.Count} Languages",
                Points = languagePoints,
                ItemsAr = profile.Languages?.Select(l => l.Language?.NameAr ?? string.Empty).ToList() ?? [],
                ItemsEn = profile.Languages?.Select(l => l.Language?.NameEn ?? string.Empty).ToList() ?? []
            });

        var certificatePoints =
            CertificatesPointsCalculator.Calculate(profile.Achievements, jobPointsMain);
        if (certificatePoints > 0)
            details.Add(new PointDetailDto
            {
                Section = "Certificates",
                SectionAr = "الشهادات",
                SectionEn = "Certificates",
                Description = $"{profile.Achievements?.Count} Certificates",
                DescriptionAr = $"{profile.Achievements?.Count} شهادات",
                DescriptionEn = $"{profile.Achievements?.Count} Certificates",
                Points = certificatePoints,
                ItemsAr = profile.Achievements?.Select(a => a.Description ?? string.Empty).ToList() ?? [],
                ItemsEn = profile.Achievements?.Select(a => a.Description ?? string.Empty).ToList() ?? []
            });

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
            certificatePoints)
        {
            Details = details
        };
    }
}
