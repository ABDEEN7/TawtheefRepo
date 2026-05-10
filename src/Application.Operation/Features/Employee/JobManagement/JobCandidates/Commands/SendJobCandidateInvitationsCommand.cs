using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Commands;

public sealed record SendJobCandidateInvitationsCommand(
    Guid JobId,
    JobCandidatesFilter? Filter,
    IReadOnlyCollection<Guid>? ApplicantIds)
    : IRequest<IResult<SendJobCandidateInvitationsResult>>;

