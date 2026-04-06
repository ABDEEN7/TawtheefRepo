using Application.Operation.Features.Employee.MinisterOffice.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.MinisterOffice.Commands;

public record UpdateMinisterOfficeCandidatePhoneCommand(
    Guid CandidateId,
    UpdateMinisterOfficeCandidatePhoneRequest Request)
    : IRequest<IResult<Unit>>;
