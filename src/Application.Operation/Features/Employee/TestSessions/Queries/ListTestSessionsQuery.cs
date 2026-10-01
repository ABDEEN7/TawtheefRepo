using Application.Operation.Features.Employee.TestSessions.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.TestSessions.Queries;

public sealed record ListTestSessionsQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<TestSessionListItemDto>>>
{
    public string? SearchText { get; init; }
    public Guid? ExamId { get; init; }
    public Guid? JobId { get; init; }
    public Guid? RoomId { get; init; }
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
    public Guid? PeriodId { get; init; }
    public Guid? StatusId { get; init; }
    public Guid? NationalityId { get; init; }
    public Guid? GenderId { get; init; }
}
