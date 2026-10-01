using Application.Operation.Features.Employee.TestSessions.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.TestSessions.Queries;

public sealed record GetReadyTestSlotsQuery : PaginatedRequest, IRequest<IResult<ReadyTestSlotsDto>>
{
    public Guid ExamId { get; init; }
    public int SelectedCandidateCount { get; init; }
    public string? SearchText { get; init; }
    public Guid? RoomId { get; init; }
    public DateOnly? Date { get; init; }
}
