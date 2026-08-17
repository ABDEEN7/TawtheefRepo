using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Export;
using Tawtheef.Domain.Entities.Users;

using Application.Operation.Features.Employee.CandidateUsers.Contracts;

namespace Application.Operation.Features.Employee.CandidateUsers.Queries;

public sealed record ExportCandidateUsersQuery
    : ICandidateUsersFilter, IRequest<IResult<FileExportResult>>
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string? Qid { get; init; }
    public string? MobileNumber { get; init; }
    public UserProfileStatus? ProfileStatus { get; init; }
    public int? Year { get; init; }
    public CandidateUsersResultScope Scope { get; init; } = CandidateUsersResultScope.Default;
}
