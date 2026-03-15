using MediatR;
using FluentResults;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs;

namespace Application.Operation.Features.Employee.CandidateUsers.Queries;

public record GetCandidateUserProfileQuery(Guid UserId) : IRequest<Result<GetProfileApprovalDetailDto>>;
