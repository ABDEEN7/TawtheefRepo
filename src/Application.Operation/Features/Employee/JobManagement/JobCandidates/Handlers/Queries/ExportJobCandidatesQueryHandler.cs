using System.Text;
using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.JobCandidates.DTOs;
using Application.Operation.Features.Employee.JobCandidates.Models;
using Application.Operation.Features.Employee.JobCandidates.Queries;
using Application.Operation.Features.Employee.JobCandidates.Services.Interfaces;
using Application.Operation.Features.Employee.JobCandidates.Utilities;
using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Features.Employee.JobCandidates.Handlers.Queries;

public sealed class ExportJobCandidatesQueryHandler(
    IUnitOfWork unitOfWork,
    IUserProfileRepository  userProfileRepository,
    IJobRepository jobRepository,
    IJobTargetCandidateCalculatorService jobTargetCandidateCalculatorService,
    IJobRequirementsService  jobRequirementsService,
    IJobCandidatesQueryBuilderService  jobCandidatesQueryBuilderService,
    ILocalizationService localizationService)
    : IQueryHandler<ExportJobCandidatesQuery, IResult<JobCandidatesExportResult>>
{
    public async Task<IResult<JobCandidatesExportResult>> Handle(
        ExportJobCandidatesQuery request,
        CancellationToken cancellationToken)
    {
        var job = await jobRepository.LoadJobWithPointsAsync(request.JobId);
        if (job is null)
            return Result.Fail<JobCandidatesExportResult>(JobMessages.JobNotFound);

        var targetCount = await jobTargetCandidateCalculatorService.GetTargetCountAsync(job.JobCategoryId,job.NumberOfVacancies);
        var req = await jobRequirementsService.GetAsync(job.MajorId,job.SubMajorId);

        var baseQuery = jobCandidatesQueryBuilderService.BuildEligibleQuery(
            job.Id,job.GenderId,job.MaximumAge,job.MinimumAge, req, request.Filter);

        var windowSize = Math.Max(targetCount * 10, 1000);

        var window = await baseQuery
            .OrderByDescending(c => c.CreatedDate)
            .Take(windowSize)
            .ToListAsync(cancellationToken);

        if (window.Count == 0)
            return Result.Ok(EmptyCsvResult(request.JobId));

        var ids = window.Select(x => x.ApplicantId).Distinct().ToList();
        var profiles = await userProfileRepository.LoadForScoringAsync(ids);
        var profileMap = profiles.ToDictionary(p => p.UserId);

        var scored = new List<JobCandidateRecord>(window.Count);
        foreach (var c in window)
        {
            if (!profileMap.TryGetValue(c.ApplicantId, out var p))
                continue;

            var major = p.Qualifications?
                .OrderByDescending(q => q.GraduationYear)
                .Select(q => q.Major)
                .FirstOrDefault();

            var candidate = c with { Applicant = p.User, Profile = p, Major = major };
            var points = JobCandidatePointsCalculator.Calculate(candidate, job.JobPoints);

            scored.Add(candidate with { Points = points });
        }

        if (request.Filter?.MinimumPoints is { } minPoints)
            scored = scored.Where(x => x.Points >= minPoints).ToList();

        var sorted = scored
            .OrderByDescending(x => x.Points)
            .ThenByDescending(x => x.CreatedDate)
            .ToList();

        var settings = await unitOfWork.GetEntityRepository<JobCandidateFilterSetting>().DbSet
            .AsNoTracking()
            .Include(s => s.CandidateTypePercentages)
            .Include(s => s.NationalityPercentages)
            .FirstOrDefaultAsync(s => s.JobId == request.JobId, cancellationToken);

        var finalList = JobCandidatesFilterUtility.ApplyPercentageFilters(sorted, settings, targetCount);

        // Export CSV
        var csv = new StringBuilder();
        csv.AppendLine("Candidate Name,Department,Job Category,Candidate Category,Major,Gender,Points");

        foreach (var candidate in finalList)
        {
            var name = EscapeCsv(localizationService.GetLocalizedFullName(candidate.Applicant));
            var department = EscapeCsv(localizationService.GetLocalizedName(job.Department));
            var jobCategory = EscapeCsv(localizationService.GetLocalizedName(job.JobCategory));
            var category = EscapeCsv(localizationService.GetLocalizedName(candidate.Profile?.CandidateType));
            var major = EscapeCsv(localizationService.GetLocalizedName(candidate.Major));
            var gender = EscapeCsv(localizationService.GetLocalizedName(candidate.Profile?.Gender));

            csv.AppendLine(string.Join(',', new[]
            {
                name, department, jobCategory, category, major, gender, candidate.Points.ToString()
            }));
        }

        return Result.Ok(new JobCandidatesExportResult
        {
            Content = Encoding.UTF8.GetBytes(csv.ToString()),
            FileName = $"job-candidates-{request.JobId:N}.csv",
            ContentType = "text/csv"
        });
    }

    private static JobCandidatesExportResult EmptyCsvResult(Guid jobId)
    {
        var csv = "Candidate Name,Department,Job Category,Candidate Category,Major,Gender,Points\n";
        return new JobCandidatesExportResult
        {
            Content = Encoding.UTF8.GetBytes(csv),
            FileName = $"job-candidates-{jobId:N}.csv",
            ContentType = "text/csv"
        };
    }

    private static string EscapeCsv(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        var escaped = value.Replace("\"", "\"\"");
        return $"\"{escaped}\"";
    }
}
