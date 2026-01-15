using Application.Operation.Features.Authenticator.DTOs;
using Application.Operation.Features.Authenticator.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Domain.Constants;

namespace Application.Operation.Features.Authenticator.Handlers.Queries;

public class GetOperationProfileHandler(IUserRepository userRepository, IMediaUrlResolver generator)
    : IQueryHandler<GetOperationProfileQuery, IResult<GetOperationProfileDto>>
{

    public async Task<IResult<GetOperationProfileDto>> Handle(GetOperationProfileQuery request, CancellationToken cancellationToken)
    {
        var userResult = await userRepository.GetByIdAsync(request.UserId);
        if (userResult.IsFailed)
            return Result.Fail<GetOperationProfileDto>(userResult.Errors);
        if (userResult.Value is null)
            return Result.Fail<GetOperationProfileDto>(ErrorsCodes.UserNotFound);

        var user = userResult.Value;
        return Result.Ok(new GetOperationProfileDto
        {
            UserId = user.Id,
            Email = user.Email!,
            Avatar = generator.ResolveAbsolute(user.Avatar),
            FullName = user.FullNameEn,
            PhoneNumber =  user.PhoneNumber ?? string.Empty
        });
    }
}
