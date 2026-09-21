using Application.Operation.Features.Employee.Interview.Evaluation.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.Evaluation.Queries;

// Always resolves to "my own form" - the caller's InterviewCommitteeMember row for the appointment's
// committee is derived from the current user, never passed by the caller.
public sealed record GetMemberEvaluationFormQuery(Guid AppointmentId) : IRequest<IResult<MemberEvaluationFormDto>>;
