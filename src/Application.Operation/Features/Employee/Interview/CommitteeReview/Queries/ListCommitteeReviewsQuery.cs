using Application.Operation.Features.Employee.Interview.CommitteeReview.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Interview.CommitteeReview.Queries;

public sealed record ListCommitteeReviewsQuery : IRequest<IResult<List<CommitteeReviewListItemDto>>>;
