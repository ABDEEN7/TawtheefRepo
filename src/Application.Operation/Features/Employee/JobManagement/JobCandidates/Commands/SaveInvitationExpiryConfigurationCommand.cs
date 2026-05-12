using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Commands;

public sealed record SaveInvitationExpiryConfigurationCommand(InvitationExpiryConfigurationRequestDto Request)
    : IRequest<IResult<InvitationExpiryConfigurationResponseDto>>;
