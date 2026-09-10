using System;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Interview.Committee.Commands;

public sealed record CreateCommitteeCommand(
    Guid JobId,
    Guid InterviewTemplateId,
    string NameAr,
    string? NameEn,
    string? ScopeDescription,
    string? Notes) : IRequest<IResult<Guid>>;
