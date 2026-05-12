using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Queries;

public sealed record GetInvitationExpiryConfigurationQuery
    : IRequest<IResult<InvitationExpiryConfigurationResponseDto>>;
