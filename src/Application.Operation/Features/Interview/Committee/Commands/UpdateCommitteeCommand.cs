using System;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Interview.Committee.Commands;

public sealed record UpdateCommitteeCommand(
    Guid Id,
    string NameAr,
    string? NameEn,
    string? ScopeDescription,
    string? Notes) : IRequest<IResult<Unit>>;
