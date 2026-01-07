using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Models;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Utilities;

internal static class JobCandidatePointsCalculator
{
    public static int Calculate(JobCandidateRecord candidate, JobPointsMain? jobPoints)
    {
        if (jobPoints == null || candidate.Profile == null)
            return 0;

        UserProfile profile = candidate.Profile;

        return ApplicantCategoryPointsCalculator.Calculate(profile.CandidateType, jobPoints)
               + EducationPointsCalculator.Calculate(profile.Qualifications, jobPoints)
               + ExperiencePointsCalculator.Calculate(profile.Experiences, jobPoints)
               + TrainingPointsCalculator.Calculate(profile.TrainingCourses, jobPoints)
               + SkillPointsCalculator.Calculate(profile.Skills, jobPoints)
               + LanguagePointsCalculator.Calculate(profile.Languages, jobPoints)
               + CertificatesPointsCalculator.Calculate(profile.Achievements, jobPoints);
    }
}
