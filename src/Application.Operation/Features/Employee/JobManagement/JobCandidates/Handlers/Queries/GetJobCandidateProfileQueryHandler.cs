using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Queries;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Utilities;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Constants;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Handlers.Queries;

public sealed class GetJobCandidateProfileQueryHandler(
    IJobRepository jobRepository,
    IUserProfileRepository userProfileRepository,
    ILocalizationService localizationService,
    IAppLogger logger)
    : IRequestHandler<GetJobCandidateProfileQuery, IResult<JobCandidateProfileDto>>
{
    public async Task<IResult<JobCandidateProfileDto>> Handle(
        GetJobCandidateProfileQuery request,
        CancellationToken cancellationToken)
    {
        var job = await jobRepository.LoadJobWithPointsAsync(request.JobId);
        if (job is null)
            return Result.Fail<JobCandidateProfileDto>(JobMessages.JobNotFound);

        if (job.JobPoints is null)
            return Result.Fail<JobCandidateProfileDto>(JobMessages.JobPointsNotFound);

        var profile = await userProfileRepository.LoadForScoringSingleAsync(request.CandidateId,cancellationToken);
        if (profile is null)
            return Result.Fail<JobCandidateProfileDto>(ErrorsCodes.UserProfileNotFound);

        var candidateRecord = new JobCandidateRecord
        {
            ApplicantId = profile.UserId,
            Applicant = profile.User,
            Profile = profile,
            JobId = job.Id,
            Major = null,
            InvitationId = null,
            InvitationStatusId = null,
            Points = 0,
            CreatedDate = profile.CreatedDate
        };

        var breakdown = JobCandidatePointsCalculator.CalculateBreakdown(
            candidateRecord,
            job.JobPoints,
            job.JobDegrees,
            job.MajorId,
            job.SubMajorId,
            logger);

        var response = new JobCandidateProfileDto
        {
            CandidateId = profile.UserId,
            CandidateName = localizationService.GetLocalizedFullName(profile.User),
            Email = profile.User?.Email,
            PhoneNumber = profile.User?.PhoneNumber,
            NationalNumber = profile.NationalNumber,
            Age = profile.Age,
            CandidateType = localizationService.GetLocalizedName(profile.CandidateType),
            Gender = localizationService.GetLocalizedName(profile.Gender),
            Nationality = localizationService.GetLocalizedName(profile.Nationality),
            ExperienceYears = Math.Round(profile.CalculatedExperienceYears, 1),
            Points = new JobCandidatePointsBreakdownDto
            {
                CategoryPoints = breakdown.CategoryPoints,
                EducationPoints = breakdown.EducationPoints,
                ExperiencePoints = breakdown.ExperiencePoints,
                TrainingPoints = breakdown.TrainingPoints,
                SkillPoints = breakdown.SkillPoints,
                LanguagePoints = breakdown.LanguagePoints,
                CertificatePoints = breakdown.CertificatePoints,
                TotalPoints = breakdown.TotalPoints
            }
        };

        return Result.Ok(response);
    }
}

