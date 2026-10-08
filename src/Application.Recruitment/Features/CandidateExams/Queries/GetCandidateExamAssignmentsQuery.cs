using Application.Recruitment.Features.CandidateExams.DTOs;
using FluentResults;
using MediatR;

namespace Application.Recruitment.Features.CandidateExams.Queries;

public sealed record GetCandidateExamAssignmentsQuery(Guid UserId)
    : IRequest<IResult<List<CandidateExamAssignmentDto>>>;
