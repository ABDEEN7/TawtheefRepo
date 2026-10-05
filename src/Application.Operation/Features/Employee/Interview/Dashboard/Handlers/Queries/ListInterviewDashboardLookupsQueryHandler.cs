using Application.Operation.Features.Employee.Interview.Dashboard.DTOs;
using Application.Operation.Features.Employee.Interview.Dashboard.Queries;
using Application.Operation.Features.Employee.Interview.Dashboard.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Interview.Dashboard.Handlers.Queries;

// Search-driven, capped feeds for the Job / Committee / Schedule filter dropdowns (no paging - the
// dropdown is narrowed by typing). `Id` resolves one entry so a filter restored from the URL can show
// its label. An evaluation-only user (no unscoped interview section) is offered only their own jobs.
public sealed class ListInterviewDashboardLookupsQueryHandler(
    IUnitOfWork unitOfWork,
    InterviewDashboardAccessResolver accessResolver)
    : IRequestHandler<ListInterviewDashboardLookupsQuery, IResult<List<InterviewDashboardLookupDto>>>
{
    private const int MaxResults = 30;

    public async Task<IResult<List<InterviewDashboardLookupDto>>> Handle(
        ListInterviewDashboardLookupsQuery request, CancellationToken cancellationToken)
    {
        var access = await accessResolver.ResolveAsync(cancellationToken);
        if (!access.Any)
            return Result.Ok(new List<InterviewDashboardLookupDto>());

        var hasUnscopedSection = access.Templates || access.Committees || access.Results
            || (access.Execution && access.ExecutionJobIds is null);
        var allowedJobIds = hasUnscopedSection ? null : access.ExecutionJobIds ?? [];
        var term = request.Search?.Trim();

        var items = request.Kind switch
        {
            InterviewDashboardLookupKind.Committee => await CommitteesAsync(request, allowedJobIds, term, cancellationToken),
            InterviewDashboardLookupKind.Schedule => await SchedulesAsync(request, allowedJobIds, term, cancellationToken),
            _ => await JobsAsync(request, allowedJobIds, term, cancellationToken)
        };

        return Result.Ok(items);
    }

    private Task<List<InterviewDashboardLookupDto>> JobsAsync(
        ListInterviewDashboardLookupsQuery request, IReadOnlyCollection<Guid>? allowedJobIds, string? term, CancellationToken ct)
    {
        var committees = Set<InterviewCommittee>();
        var schedules = Set<InterviewSchedule>();
        var query = Set<Job>().Where(j => committees.Any(c => c.JobId == j.Id) || schedules.Any(s => s.JobId == j.Id));

        if (allowedJobIds is not null)
            query = query.Where(j => allowedJobIds.Contains(j.Id));
        if (request.Id.HasValue)
            query = query.Where(j => j.Id == request.Id.Value);
        else if (!string.IsNullOrEmpty(term))
            query = query.Where(j => j.JobTitle!.JobNameAr.Contains(term)
                || (j.JobTitle.JobNameEn != null && j.JobTitle.JobNameEn.Contains(term)));

        return query
            .OrderBy(j => j.JobTitle!.JobNameAr)
            .Take(MaxResults)
            .Select(j => new InterviewDashboardLookupDto(j.Id, j.JobTitle!.JobNameAr, j.JobTitle.JobNameEn, null, null))
            .ToListAsync(ct);
    }

    private Task<List<InterviewDashboardLookupDto>> CommitteesAsync(
        ListInterviewDashboardLookupsQuery request, IReadOnlyCollection<Guid>? allowedJobIds, string? term, CancellationToken ct)
    {
        var query = Set<InterviewCommittee>();

        if (allowedJobIds is not null)
            query = query.Where(c => allowedJobIds.Contains(c.JobId));
        if (request.JobId.HasValue)
            query = query.Where(c => c.JobId == request.JobId.Value);
        if (request.Id.HasValue)
            query = query.Where(c => c.Id == request.Id.Value);
        else if (!string.IsNullOrEmpty(term))
            query = query.Where(c => c.NameAr.Contains(term) || (c.NameEn != null && c.NameEn.Contains(term)) || c.Code.Contains(term));

        return query
            .OrderBy(c => c.NameAr)
            .Take(MaxResults)
            .Select(c => new InterviewDashboardLookupDto(
                c.Id, c.NameAr, c.NameEn, c.Job!.JobTitle!.JobNameAr, c.Job.JobTitle.JobNameEn))
            .ToListAsync(ct);
    }

    private Task<List<InterviewDashboardLookupDto>> SchedulesAsync(
        ListInterviewDashboardLookupsQuery request, IReadOnlyCollection<Guid>? allowedJobIds, string? term, CancellationToken ct)
    {
        var query = Set<InterviewSchedule>();

        if (allowedJobIds is not null)
            query = query.Where(s => allowedJobIds.Contains(s.JobId));
        if (request.JobId.HasValue)
            query = query.Where(s => s.JobId == request.JobId.Value);
        if (request.Id.HasValue)
            query = query.Where(s => s.Id == request.Id.Value);
        else if (!string.IsNullOrEmpty(term))
            query = query.Where(s => s.TitleAr.Contains(term) || (s.TitleEn != null && s.TitleEn.Contains(term)));

        return query
            .OrderByDescending(s => s.CreatedDate)
            .Take(MaxResults)
            .Select(s => new InterviewDashboardLookupDto(
                s.Id, s.TitleAr, s.TitleEn, s.Job!.JobTitle!.JobNameAr, s.Job.JobTitle.JobNameEn))
            .ToListAsync(ct);
    }

    private IQueryable<T> Set<T>() where T : Tawtheef.Domain.Common.EventEntity =>
        unitOfWork.GetEntityRepository<T>().DbSet.AsNoTracking();
}
