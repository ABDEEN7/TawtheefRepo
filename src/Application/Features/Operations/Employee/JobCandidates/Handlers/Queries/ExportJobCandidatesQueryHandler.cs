using System.Text;
using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Queries;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Services;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Handlers.Queries;

public sealed class ExportJobCandidatesQueryHandler(
    IUnitOfWork unitOfWork,
    ILocalizationService localizationService,
    IJobPointsRepository jobPointsRepository)
    : IRequestHandler<ExportJobCandidatesQuery, IResult<JobCandidatesExportResult>>
{
    private readonly JobCandidatePointsCalculator _pointsCalculator = new();

    public async Task<IResult<JobCandidatesExportResult>> Handle(
        ExportJobCandidatesQuery request,
        CancellationToken cancellationToken)
    {
        var query = JobCandidatesQueryBuilder.Build(unitOfWork, request.JobId, request.Filter);

        if (request.InvitationIds is { Count: > 0 })
        {
            query = query.Where(candidate =>
                candidate.InvitationId.HasValue &&
                request.InvitationIds.Contains(candidate.InvitationId.Value));
        }

        var candidates = await query.ToListAsync(cancellationToken);
        var jobPoints = await LoadJobPointsAsync(request.JobId, cancellationToken);

        var job = await unitOfWork.GetEntityRepository<Domain.Entities.Recruitment.Job>().DbSet
            .AsNoTracking()
            .Include(j => j.Department)
            .Include(j => j.JobCategory)
            .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);

        var candidatesWithPoints = candidates
            .Select(candidate => candidate with { Points = _pointsCalculator.Calculate(candidate, jobPoints) })
            .ToList();

        if (request.Filter?.MinimumPoints is { } minPoints)
        {
            candidatesWithPoints = candidatesWithPoints
                .Where(candidate => candidate.Points >= minPoints)
                .ToList();
        }

        var sortedCandidates = candidatesWithPoints
            .OrderByDescending(candidate => candidate.CreatedDate ?? DateTime.MinValue)
            .ToList();

        var csv = new StringBuilder();
        csv.AppendLine("Candidate Name,Department,Job Category,Candidate Category,Major,Gender,Points");

        foreach (var candidate in sortedCandidates)
        {
            var name = EscapeCsv(localizationService.GetLocalizedFullName(candidate.Applicant));
            var department = EscapeCsv(localizationService.GetLocalizedName(job?.Department));
            var jobCategory = EscapeCsv(localizationService.GetLocalizedName(job?.JobCategory));
            var category = EscapeCsv(localizationService.GetLocalizedName(candidate.Profile?.CandidateType));
            var major = EscapeCsv(localizationService.GetLocalizedName(candidate.Major));
            var gender = EscapeCsv(localizationService.GetLocalizedName(candidate.Profile?.Gender));

            csv.AppendLine(string.Join(',', new[]
            {
                name,
                department,
                jobCategory,
                category,
                major,
                gender,
                candidate.Points.ToString()
            }));
        }

        var result = new JobCandidatesExportResult
        {
            Content = Encoding.UTF8.GetBytes(csv.ToString()),
            FileName = $"job-candidates-{request.JobId:N}.csv",
            ContentType = "text/csv"
        };

        return Result.Ok(result);
    }

    private async Task<JobPointsMain?> LoadJobPointsAsync(Guid jobId, CancellationToken cancellationToken)
    {
        var result = await jobPointsRepository.GetByJobIdAsync(jobId);
        return result.IsSuccess ? result.Value : null;
    }

    private static string EscapeCsv(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var escaped = value.Replace("\"", "\"\"");
        return $"\"{escaped}\"";
    }
}
