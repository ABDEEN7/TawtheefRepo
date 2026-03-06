using MediatR;
using FluentResults;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Application.Operation.Features.Employee.JobManagement.Job.Queries;

public record GetLatestJobReviewAttachmentQuery(Guid JobId) : IRequest<IResult<FileRefDto?>>;

