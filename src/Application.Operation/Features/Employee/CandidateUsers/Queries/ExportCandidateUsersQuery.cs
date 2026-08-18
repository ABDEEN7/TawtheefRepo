using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Export;
using Tawtheef.Domain.Entities.Users;

using Application.Operation.Features.Employee.CandidateUsers.Contracts;

namespace Application.Operation.Features.Employee.CandidateUsers.Queries;

public sealed record ExportCandidateUsersQuery
    : ICandidateUsersFilter, IRequest<IResult<FileExportResult>>
{
    public string? Search { get; init; }
    public bool? IsBlocked { get; init; }
    public List<UserProfileStatus>? ProfileStatuses { get; init; }
    public UserProfileStatus? ProfileStatus { get; init; }
    public int? Year { get; init; }
    public CandidateUsersResultScope Scope { get; init; } = CandidateUsersResultScope.Default;
}
