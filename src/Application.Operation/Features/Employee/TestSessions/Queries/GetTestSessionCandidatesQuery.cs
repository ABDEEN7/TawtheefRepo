using Application.Operation.Features.Employee.TestSessions.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Domain.Entities.Exams;

namespace Application.Operation.Features.Employee.TestSessions.Queries;

public sealed record GetTestSessionCandidatesQuery : IRequest<IResult<TestSessionCandidatesDto>>
{
    public Guid ExamId { get; init; }
    public string Language { get; init; } = "en";
    public TestSessionGenderFilter? GenderFilter { get; init; }
    public TestSessionNationalityFilter? NationalityFilter { get; init; }
}
