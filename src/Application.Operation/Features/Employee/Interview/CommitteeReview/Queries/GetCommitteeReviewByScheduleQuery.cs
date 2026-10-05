using Application.Operation.Features.Employee.Interview.CommitteeReview.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.CommitteeReview.Queries;

public sealed record GetCommitteeReviewByScheduleQuery(Guid ScheduleId) : IRequest<IResult<CommitteeReviewDto>>;
