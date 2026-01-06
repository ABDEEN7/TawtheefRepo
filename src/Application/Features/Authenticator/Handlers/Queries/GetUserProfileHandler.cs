using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.DTOs;
using Tawtheef.Application.Features.Authenticator.Queries;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Queries;

public class GetUserProfileHandler(IUserRepository userRepository, IMediaUrlResolver generator)
    : IQueryHandler<GetUserProfileQuery, IResult<GetUserProfileDto>>
{

    public async Task<IResult<GetUserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var userResult = await userRepository.GetByIdAsync(request.UserId);
        if (userResult.IsFailed)
            return Result.Fail<GetUserProfileDto>(userResult.Errors);
        if (userResult.Value is null)
            return Result.Fail<GetUserProfileDto>(ErrorsCodes.UserNotFound);

        var user = userResult.Value;
        return Result.Ok(new GetUserProfileDto
        {
            UserId = user.Id,
            Email = user.Email!,
            Avatar = generator.ResolveAbsolute(user.Avatar),
            FullName = user.FullNameEn,
            PhoneNumber =  user.PhoneNumber ?? string.Empty,
            AgreedToTerms = user.AgreedToTerms
        });
    }
}
