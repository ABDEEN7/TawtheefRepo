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
    : IRequestHandler<GetLatestJobReviewAttachmentQuery, IResult<List<FileRefDto>>>
{
    public async Task<IResult<List<FileRefDto>>> Handle(GetLatestJobReviewAttachmentQuery request, CancellationToken cancellationToken)
    {
        var result = await jobReviewAttachmentRepository.GetLastReviewCycleAsync(request.JobId);

        if (result.IsFailed)
            return Result.Fail<List<FileRefDto>>(result.Errors);

        if (result.Value is null || !result.Value.Any())
            return Result.Ok(new List<FileRefDto>());

        using var scope = new MapContextScope();
        scope.Context.Parameters[ResourceMapper.MediaKey] = media;

        var attachmentsDto = mapper.Map<List<FileRefDto>>(result.Value.Select(x => x.Attachment).Where(a => a != null).ToList());
        return Result.Ok(attachmentsDto);
    }
}

