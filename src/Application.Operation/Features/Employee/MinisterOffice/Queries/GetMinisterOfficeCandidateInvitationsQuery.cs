using Application.Operation.Features.Employee.MinisterOffice.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.MinisterOffice.Queries;

public record GetMinisterOfficeCandidateInvitationsQuery(Guid CandidateId)
    : IRequest<IResult<IReadOnlyList<MinisterOfficeCandidateInvitationDto>>>;
