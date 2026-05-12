using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Queries;
using FluentResults;
using MapsterMapper;
using MediatR;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Handlers.Queries;

public sealed class GetInvitationExpiryConfigurationQueryHandler(
    IInvitationExpiryConfigurationRepository repository,
    IMapper mapper)
    : IRequestHandler<GetInvitationExpiryConfigurationQuery, IResult<InvitationExpiryConfigurationResponseDto>>
{
    public async Task<IResult<InvitationExpiryConfigurationResponseDto>> Handle(
        GetInvitationExpiryConfigurationQuery request,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetAsync();
        if (result.IsFailed)
            return Result.Fail<InvitationExpiryConfigurationResponseDto>(result.Errors);

        return Result.Ok(mapper.Map<InvitationExpiryConfigurationResponseDto>(result.Value));
    }
}
