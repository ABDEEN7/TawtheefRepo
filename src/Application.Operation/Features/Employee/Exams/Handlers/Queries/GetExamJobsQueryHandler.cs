using Application.Operation.Features.Employee.Exams.DTOs;
using Application.Operation.Features.Employee.Exams.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Exams.Handlers.Queries;

public sealed class GetExamJobsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetExamJobsQuery, IResult<List<ExamJobDto>>>
{
    public async Task<IResult<List<ExamJobDto>>> Handle(GetExamJobsQuery request, CancellationToken ct)
    {
        var search = request.Search?.Trim();
        var jobs = unitOfWork.Context.Set<Job>().AsNoTracking();
        if (!string.IsNullOrEmpty(search))
            jobs = jobs.Where(x => x.JobTitle!.JobNameAr.Contains(search) ||
                                   x.JobTitle.JobNameEn.Contains(search) || x.JobTitle.JobNumber.Contains(search));
        var rows = await jobs.OrderByDescending(x => x.CreatedDate).ThenBy(x => x.Id).Take(50)
            .Select(x => new
            {
                x.Id,
                NameAr = x.JobTitle!.JobNameAr,
                NameEn = x.JobTitle.JobNameEn,
                x.JobTitle.JobNumber,
                ManagementAr = x.Management!.NameAr,
                ManagementEn = x.Management.NameEn,
                DepartmentAr = x.Department == null ? null : x.Department.NameAr,
                DepartmentEn = x.Department == null ? null : x.Department.NameEn,
                MajorAr = x.Major == null ? null : x.Major.NameAr,
                MajorEn = x.Major == null ? null : x.Major.NameEn,
                SubMajorAr = x.SubMajor == null ? null : x.SubMajor.NameAr,
                SubMajorEn = x.SubMajor == null ? null : x.SubMajor.NameEn,
                SpecializationsAr = x.JobSpecializations.OrderBy(s => s.Id)
                    .Select(s => s.Major!.NameAr + " / " + s.SubMajor!.NameAr).ToList(),
                SpecializationsEn = x.JobSpecializations.OrderBy(s => s.Id)
                    .Select(s => s.Major!.NameEn + " / " + s.SubMajor!.NameEn).ToList()
            }).ToListAsync(ct);
        if (request.IncludeJobId is { } includeJobId && rows.All(x => x.Id != includeJobId))
        {
            var selectedJob = await unitOfWork.Context.Set<Job>().AsNoTracking()
                .Where(x => x.Id == includeJobId)
                .Select(x => new
                {
                    x.Id,
                    NameAr = x.JobTitle!.JobNameAr,
                    NameEn = x.JobTitle.JobNameEn,
                    x.JobTitle.JobNumber,
                    ManagementAr = x.Management!.NameAr,
                    ManagementEn = x.Management.NameEn,
                    DepartmentAr = x.Department == null ? null : x.Department.NameAr,
                    DepartmentEn = x.Department == null ? null : x.Department.NameEn,
                    MajorAr = x.Major == null ? null : x.Major.NameAr,
                    MajorEn = x.Major == null ? null : x.Major.NameEn,
                    SubMajorAr = x.SubMajor == null ? null : x.SubMajor.NameAr,
                    SubMajorEn = x.SubMajor == null ? null : x.SubMajor.NameEn,
                    SpecializationsAr = x.JobSpecializations.OrderBy(s => s.Id)
                        .Select(s => s.Major!.NameAr + " / " + s.SubMajor!.NameAr).ToList(),
                    SpecializationsEn = x.JobSpecializations.OrderBy(s => s.Id)
                        .Select(s => s.Major!.NameEn + " / " + s.SubMajor!.NameEn).ToList()
                }).FirstOrDefaultAsync(ct);
            if (selectedJob != null) rows.Insert(0, selectedJob);
        }
        return Result.Ok(rows.Select(x => new ExamJobDto(x.Id, x.NameAr, x.NameEn, x.JobNumber,
            x.ManagementAr, x.ManagementEn, x.DepartmentAr, x.DepartmentEn,
            Specializations(x.MajorAr, x.SubMajorAr, x.SpecializationsAr),
            Specializations(x.MajorEn, x.SubMajorEn, x.SpecializationsEn))).ToList());
    }

    private static List<string> Specializations(string? major, string? subMajor, List<string> additional)
    {
        var primary = string.Join(" / ", new[] { major, subMajor }.Where(x => !string.IsNullOrWhiteSpace(x)));
        if (!string.IsNullOrWhiteSpace(primary)) additional.Insert(0, primary);
        return additional.Distinct().ToList();
    }
}
