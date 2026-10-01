using Application.Operation.Features.Employee.TestSessions.DTOs;
using Application.Operation.Features.Employee.TestSessions.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.TestSessions.Handlers.Queries;

public sealed class GetTestSessionExamDetailsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetTestSessionExamDetailsQuery, IResult<TestSessionExamDetailsDto>>
{
    public async Task<IResult<TestSessionExamDetailsDto>> Handle(GetTestSessionExamDetailsQuery request,
        CancellationToken ct)
    {
        var isArabic = string.Equals(request.Language, "ar", StringComparison.OrdinalIgnoreCase);
        var exam = await unitOfWork.Context.Set<Exam>().AsNoTracking()
            .Where(x => x.Id == request.ExamId && x.StatusId == ExamStatusIds.Approved)
            .Select(x => new
            {
                x.Id,
                ExamName = isArabic ? x.TitleAr : x.TitleEn ?? x.TitleAr,
                JobTitle = isArabic ? x.Job!.JobTitle!.JobNameAr : x.Job!.JobTitle!.JobNameEn,
                Specialization = x.Job!.SubMajor != null
                    ? (isArabic ? x.Job.SubMajor.NameAr : x.Job.SubMajor.NameEn)
                    : x.Job.Major == null
                        ? null
                        : (isArabic ? x.Job.Major.NameAr : x.Job.Major.NameEn),
                MainSpecialization = x.Job!.Major == null
                    ? null
                    : (isArabic ? x.Job.Major.NameAr : x.Job.Major.NameEn),
                SubSpecialization = x.Job.SubMajor == null
                    ? null
                    : (isArabic ? x.Job.SubMajor.NameAr : x.Job.SubMajor.NameEn),
                ExamStatus = isArabic ? x.Status!.NameAr : x.Status!.NameEn,
                TotalCandidates = unitOfWork.Context.Set<Invitation>().Count(i => i.JobId == x.JobId),
                DurationMinutes = unitOfWork.Context.Set<ExamPart>().Where(part => part.ExamId == x.Id)
                    .Sum(part => (int?)part.DurationMinutes) ?? 0,
                NumberOfQuestions = x.TotalQuestions,
                QualificationScore = unitOfWork.Context.Set<ExamPart>().Where(part => part.ExamId == x.Id)
                    .OrderBy(part => part.PartNo).Select(part => part.QualificationScore).FirstOrDefault()
            })
            .FirstOrDefaultAsync(ct);

        if (exam is null) return Result.Fail<TestSessionExamDetailsDto>(ErrorsCodes.InvalidRequest);

        var parts = await unitOfWork.Context.Set<ExamPart>().AsNoTracking()
            .Where(part => part.ExamId == exam.Id)
            .OrderBy(part => part.PartNo)
            .Select(part => new TestSessionExamPartDto(part.PartNo, part.DurationMinutes))
            .ToListAsync(ct);

        return Result.Ok(new TestSessionExamDetailsDto(exam.Id, exam.ExamName, exam.JobTitle, exam.Specialization,
            exam.MainSpecialization, exam.SubSpecialization, exam.ExamStatus, exam.TotalCandidates,
            exam.DurationMinutes, parts, exam.NumberOfQuestions, exam.QualificationScore));
    }
}
