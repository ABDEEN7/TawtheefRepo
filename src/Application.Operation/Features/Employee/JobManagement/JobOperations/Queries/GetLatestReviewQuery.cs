using Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Queries;

public record GetLatestReviewQuery(Guid JobId) : IRequest<IResult<JobReviewResponseDto>>;

