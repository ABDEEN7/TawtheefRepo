using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Committee.Commands;

public sealed record UpdateCommitteeCommand(
    Guid Id,
    string NameAr,
    string? NameEn,
    string? ScopeDescription,
    string? Notes,
    List<MemberInputDto> Members): IRequest<IResult<Unit>>;
