using Application.Operation.Features.Employee.Exams.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Exams.Queries;

public sealed record GetExamConfigurationQuery(Guid ExamId, bool IsViewMode = false)
    : IRequest<IResult<ExamConfigurationDto>>;
