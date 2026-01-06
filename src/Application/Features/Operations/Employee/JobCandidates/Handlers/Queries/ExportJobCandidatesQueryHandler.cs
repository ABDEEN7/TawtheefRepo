using System.Text;
using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Models;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Queries;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Services;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Handlers.Queries;

public sealed class ExportJobCandidatesQueryHandler(
    IUnitOfWork unitOfWork,
    ILocalizationService localizationService)
    : IQueryHandler<ExportJobCandidatesQuery, IResult<JobCandidatesExportResult>>
{
    private readonly JobCandidatePointsCalculator _pointsCalculator = new();

    public async Task<IResult<JobCandidatesExportResult>> Handle(
        ExportJobCandidatesQuery request,
        CancellationToken cancellationToken)
    {
        var job = await unitOfWork.GetEntityRepository<Domain.Entities.Recruitment.Job>().DbSet
            .AsNoTracking()
            .Include(j => j.Department)
            .Include(j => j.JobCategory)
            .Include(j => j.JobPoints).ThenInclude(p => p!.Details)
            .Include(j => j.Major)
            .Include(j => j.SubMajor)
            .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);

        if (job is null)
            return Result.Fail<JobCandidatesExportResult>(JobMessages.JobNotFound);

        var targetCount = GetTargetCount(job);
        var req = await JobRequirementsService.GetAsync(unitOfWork, job, cancellationToken);

        var baseQuery = JobCandidatesQueryBuilder.BuildEligibleQuery(
            unitOfWork,
            job,
            req,
            request.Filter);

        var windowSize = Math.Max(targetCount * 10, 1000);

        var window = await baseQuery
            .OrderByDescending(c => c.CreatedDate)
            .Take(windowSize)
            .ToListAsync(cancellationToken);

        if (window.Count == 0)
            return Result.Ok(EmptyCsvResult(request.JobId));

        var ids = window.Select(x => x.ApplicantId).Distinct().ToList();
        var profiles = await CandidateProfileLoader.LoadForScoringAsync(unitOfWork, ids, cancellationToken);
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
            var points = _pointsCalculator.Calculate(candidate, job.JobPoints);

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

        var finalList = JobCandidatesFilterProcessor.ApplyPercentageFilters(sorted, settings, targetCount);

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

    private static int GetTargetCount(Domain.Entities.Recruitment.Job job)
    {
        var vacancies = Math.Max(1, job.NumberOfVacancies);
        return job.JobCategoryId == JobCategoryIds.Academic ? vacancies * 5 : vacancies;
    }
}
