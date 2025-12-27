using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Queries;

public record GetLatestJobReviewAttachmentQuery(Guid JobId) : IRequest<IResult<FileRefDto?>>;
