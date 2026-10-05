using Application.Operation.Features.Employee.Interview.CommitteeReview.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.CommitteeReview.Commands;

// Saves the chair's per-candidate recommendations; SendForApproval also moves the report from
// CommitteeReview on to the Approve Interview Results stage (UnderReview).
public sealed record SaveCommitteeReviewCommand(
    Guid ReportId, IReadOnlyCollection<CommitteeReviewCandidateInputDto> Candidates, bool SendForApproval)
    : IRequest<IResult<Unit>>;
