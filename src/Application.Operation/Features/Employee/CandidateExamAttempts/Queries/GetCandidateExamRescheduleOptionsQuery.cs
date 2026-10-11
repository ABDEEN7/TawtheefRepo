using Application.Operation.Features.Employee.CandidateExamAttempts.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.CandidateExamAttempts.Queries;

public sealed record GetCandidateExamRescheduleOptionsQuery(Guid TestAttemptId, Guid OfficerId)
    : IRequest<IResult<IReadOnlyList<CandidateExamRescheduleOptionDto>>>;
