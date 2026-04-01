using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Queries;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Services.Interfaces;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Utilities;
using ClosedXML.Excel;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Handlers.Queries;

public sealed class ExportJobCandidatesQueryHandler(
    IUnitOfWork unitOfWork,
    IUserProfileRepository  userProfileRepository,
    IJobRepository jobRepository,
    IJobTargetCandidateCalculatorService jobTargetCandidateCalculatorService,
    IJobRequirementsService  jobRequirementsService,
    IJobCandidatesQueryBuilderService  jobCandidatesQueryBuilderService,
    ILocalizationService localizationService,
    IAppLogger logger)
    : IRequestHandler<ExportJobCandidatesQuery, IResult<JobCandidatesExportResult>>
{
    public async Task<IResult<JobCandidatesExportResult>> Handle(
        ExportJobCandidatesQuery request,
        CancellationToken cancellationToken)
    {
        var job = await jobRepository.LoadJobWithPointsAsync(request.JobId);
        if (job is null)
            return Result.Fail<JobCandidatesExportResult>(JobMessages.JobNotFound);

        var targetCount = await jobTargetCandidateCalculatorService.GetTargetCountAsync(job.JobCategoryId,job.NumberOfVacancies);
        var req = await jobRequirementsService.GetAsync(job);

        var baseQuery = jobCandidatesQueryBuilderService.BuildEligibleQuery(
            job.Id, job.WorkLocationId, job.GenderId,job.MaximumAge,job.MinimumAge, req, request.Filter);

        var windowSize = Math.Max(targetCount * 10, 1000);

        var window = await baseQuery
            .OrderByDescending(c => c.CreatedDate)
            .Take(windowSize)
            .ToListAsync(cancellationToken);

        if (window.Count == 0)
            return Result.Ok(EmptyXlsxResult(request.JobId));

        var ids = window.Select(x => x.ApplicantId).Distinct().ToList();
        var profiles = await userProfileRepository.LoadForScoringAsync(ids,cancellationToken);
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
            if (job.JobPoints == null) 
                return Result.Fail<JobCandidatesExportResult>(JobMessages.JobPointsNotFound);
            var points = JobCandidatePointsCalculator.Calculate(candidate, job.JobPoints,job.JobDegrees,job.MajorId,job.SubMajorId,logger);

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

        // Export XLSX (ClosedXML)
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add(JobCandidatesMessages.Candidate);

        ws.RightToLeft = true;

        ws.Cell(1, 1).Value = localizationService.GetLocalizedValue(JobCandidatesMessages.CandidateName);
        ws.Cell(1, 2).Value = localizationService.GetLocalizedValue(JobCandidatesMessages.Department);
        ws.Cell(1, 3).Value = localizationService.GetLocalizedValue(JobCandidatesMessages.JobCategory);
        ws.Cell(1, 4).Value = localizationService.GetLocalizedValue(JobCandidatesMessages.CandidateCategory);
        ws.Cell(1, 5).Value = localizationService.GetLocalizedValue(JobCandidatesMessages.Major);
        ws.Cell(1, 6).Value = localizationService.GetLocalizedValue(JobCandidatesMessages.Gender);
        ws.Cell(1, 7).Value = localizationService.GetLocalizedValue(JobCandidatesMessages.Points);

        var header = ws.Range(1, 1, 1, 7);
        header.Style.Font.Bold = true;
        header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        var row = 2;
        foreach (var candidate in finalList)
        {
            ws.Cell(row, 1).Value = localizationService.GetLocalizedFullName(candidate.Applicant);
            ws.Cell(row, 2).Value = localizationService.GetLocalizedName(job.Department);
            ws.Cell(row, 3).Value = localizationService.GetLocalizedName(job.JobCategory);
            ws.Cell(row, 4).Value = localizationService.GetLocalizedName(candidate.Profile?.CandidateType);
            ws.Cell(row, 5).Value = localizationService.GetLocalizedName(candidate.Major);
            ws.Cell(row, 6).Value = localizationService.GetLocalizedName(candidate.Profile?.Gender);
            ws.Cell(row, 7).Value = candidate.Points;

            row++;
        }

        ws.SheetView.FreezeRows(1);
        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);

        return Result.Ok(new JobCandidatesExportResult
        {
            Content = ms.ToArray(),
            FileName = $"job-candidates-{request.JobId:N}.xlsx",
            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        });
    }

    private JobCandidatesExportResult EmptyXlsxResult(Guid jobId)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add(JobCandidatesMessages.Candidate);
        ws.RightToLeft = true;

        ws.Cell(1, 1).Value = localizationService.GetLocalizedValue(JobCandidatesMessages.CandidateName);
        ws.Cell(1, 2).Value = localizationService.GetLocalizedValue(JobCandidatesMessages.Department);
        ws.Cell(1, 3).Value = localizationService.GetLocalizedValue(JobCandidatesMessages.JobCategory);
        ws.Cell(1, 4).Value = localizationService.GetLocalizedValue(JobCandidatesMessages.CandidateCategory);
        ws.Cell(1, 5).Value = localizationService.GetLocalizedValue(JobCandidatesMessages.Major);
        ws.Cell(1, 6).Value = localizationService.GetLocalizedValue(JobCandidatesMessages.Gender);
        ws.Cell(1, 7).Value = localizationService.GetLocalizedValue(JobCandidatesMessages.Points);

        ws.Range(1, 1, 1, 7).Style.Font.Bold = true;
        ws.SheetView.FreezeRows(1);
        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);

        return new JobCandidatesExportResult
        {
            Content = ms.ToArray(),
            FileName = $"job-candidates-{jobId:N}.xlsx",
            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        };
    }
}

