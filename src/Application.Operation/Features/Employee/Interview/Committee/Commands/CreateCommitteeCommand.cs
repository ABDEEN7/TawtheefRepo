using System;
using System.Collections.Generic;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Committee.Commands;

public sealed record CreateCommitteeCommand(
    Guid JobId,
    Guid InterviewTemplateId,
    string NameAr,
    string? NameEn,
    string? ScopeDescription,
    string? Notes,
    List<MemberInputDto> Members) : IRequest<IResult<Guid>>;
