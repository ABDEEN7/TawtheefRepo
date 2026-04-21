using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.JobManagement.JobOperations.Queries;
using MediatR;
using FluentResults;
using Mapster;
using MapsterMapper;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Application.Common.Mappers;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Handlers.Queries;

public class GetLatestJobReviewAttachmentQueryHandler(IJobReviewAttachmentRepository jobReviewAttachmentRepository, IMediaUrlResolver media, IMapper mapper)
    : IRequestHandler<GetLatestJobReviewAttachmentQuery, IResult<FileRefDto?>>
{
    public async Task<IResult<FileRefDto?>> Handle(GetLatestJobReviewAttachmentQuery request, CancellationToken cancellationToken)
    {
        var result = await jobReviewAttachmentRepository.GetLastReviewCycleAsync(request.JobId);

        if (result.IsFailed)
            return Result.Fail<FileRefDto>(result.Errors);

        var jobAttachmentReview = result.Value.FirstOrDefault();

        if (jobAttachmentReview?.Attachment is null)
            return Result.Ok<FileRefDto?>(null);

        using var scope = new MapContextScope();
        scope.Context.Parameters[ResourceMapper.MediaKey] = media;

        var attachmentDto = mapper.Map<FileRefDto>(jobAttachmentReview.Attachment);
        return Result.Ok(attachmentDto);
    }
}

