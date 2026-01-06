using System.Text;
using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Queries;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Handlers.Queries;

public sealed class ExportJobCandidatesQueryHandler(
    IUnitOfWork unitOfWork,
    ILocalizationService localizationService)
    : IQueryHandler<ExportJobCandidatesQuery, IResult<JobCandidatesExportResult>>
{
    public async Task<IResult<JobCandidatesExportResult>> Handle(
        ExportJobCandidatesQuery request,
        CancellationToken cancellationToken)
    {
        var query = JobCandidatesQueryBuilder.Build(unitOfWork, request.JobId, request.Filter);

        if (request.InvitationIds is { Count: > 0 })
        {
            query = query.Where(candidate => request.InvitationIds.Contains(candidate.InvitationId));
        }

        var candidates = await query
            .OrderByDescending(candidate => candidate.CreatedDate)
            .ToListAsync(cancellationToken);

        var csv = new StringBuilder();
        csv.AppendLine("Candidate Name,Department,Job Category,Candidate Category,Major,Gender,Points");

        foreach (var candidate in candidates)
        {
            var name = EscapeCsv(localizationService.GetLocalizedFullName(candidate.Applicant));
            var department = EscapeCsv(localizationService.GetLocalizedName(candidate.Job?.Department));
            var jobCategory = EscapeCsv(localizationService.GetLocalizedName(candidate.Job?.JobCategory));
            var category = EscapeCsv(localizationService.GetLocalizedName(candidate.Profile?.CandidateType));
            var major = EscapeCsv(localizationService.GetLocalizedName(candidate.Major));
            var gender = EscapeCsv(localizationService.GetLocalizedName(candidate.Profile?.Gender));

            csv.AppendLine(string.Join(',', [
                name,
                department,
                jobCategory,
                category,
                major,
                gender,
                candidate.Points.ToString()
            ]));
        }

        var result = new JobCandidatesExportResult
        {
            Content = Encoding.UTF8.GetBytes(csv.ToString()),
            FileName = $"job-candidates-{request.JobId:N}.csv",
            ContentType = "text/csv"
        };

        return Result.Ok(result);
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
