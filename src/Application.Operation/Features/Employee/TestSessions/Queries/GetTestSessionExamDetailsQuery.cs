using Application.Operation.Features.Employee.TestSessions.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.TestSessions.Queries;

public sealed record GetTestSessionExamDetailsQuery(Guid ExamId, string Language)
    : IRequest<IResult<TestSessionExamDetailsDto>>;
