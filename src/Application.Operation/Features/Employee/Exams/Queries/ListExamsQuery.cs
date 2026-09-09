using Application.Operation.Features.Employee.Exams.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.Exams.Queries;

public sealed record ListExamsQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<ExamListItemDto>>>
{
    public string? Search { get; init; }
    public Guid? SpecializationId { get; init; }
    public Guid? StatusId { get; init; }
    public DateOnly? CreatedFrom { get; init; }
    public DateOnly? CreatedTo { get; init; }
}
