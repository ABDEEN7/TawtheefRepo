using Application.Operation.Features.Employee.Exams.DTOs;
using Application.Operation.Features.Employee.Exams.Queries;
using Application.Operation.Features.Employee.Exams.Services;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Exams.Handlers.Queries;

public sealed class GetExamJobSelectionQueryHandler(ExamService examService)
    : IRequestHandler<GetExamJobSelectionQuery, IResult<ExamJobSelectionDto>>
{
    public async Task<IResult<ExamJobSelectionDto>> Handle(GetExamJobSelectionQuery request, CancellationToken ct)
    {
        var hasPendingApprovalExam = await examService.HasPendingApprovalExamForJobAsync(
            request.JobId, request.ExcludeExamId, ct);
        if (hasPendingApprovalExam)
            return Result.Ok(new ExamJobSelectionDto(true, null));

        var approvedExam = await examService.GetApprovedForJobAsync(request.JobId, ct);
        return Result.Ok(new ExamJobSelectionDto(false, approvedExam));
    }
}
