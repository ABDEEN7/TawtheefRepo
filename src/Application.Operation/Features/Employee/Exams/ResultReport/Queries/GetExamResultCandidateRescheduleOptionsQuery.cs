using Application.Operation.Features.Employee.Exams.ResultReport.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Exams.ResultReport.Queries;

public sealed record GetExamResultCandidateRescheduleOptionsQuery(Guid ReportId, Guid CandidateResultId)
    : IRequest<IResult<IReadOnlyList<ExamResultCandidateRescheduleOptionDto>>>;
