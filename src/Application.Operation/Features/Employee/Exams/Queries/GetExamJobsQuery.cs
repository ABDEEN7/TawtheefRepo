using Application.Operation.Features.Employee.Exams.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Exams.Queries;

public sealed record GetExamJobsQuery(string? Search = null, Guid? IncludeJobId = null)
    : IRequest<IResult<List<ExamJobDto>>>;
