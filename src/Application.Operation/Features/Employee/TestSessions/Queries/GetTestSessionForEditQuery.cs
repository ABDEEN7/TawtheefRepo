using Application.Operation.Features.Employee.TestSessions.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.TestSessions.Queries;

public sealed record GetTestSessionForEditQuery(Guid TestSessionId, string Language, bool ViewMode = false)
    : IRequest<IResult<TestSessionEditDto>>;
