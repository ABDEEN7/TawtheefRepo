using Application.Operation.Features.Employee.TestSessions.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.TestSessions.Commands;

public sealed record SaveTestSessionSetupCommand(SaveTestSessionSetupDto Setup)
    : IRequest<IResult<SavedTestSessionSetupDto>>;
