using Application.Recruitment.Features.Authenticator.DTOs;
using Application.Recruitment.Features.Authenticator.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Constants;

namespace Application.Recruitment.Features.Authenticator.Handlers.Queries;

public class GetRecruitmentProfileHandler(IUserRepository userRepository, IMediaUrlResolver generator)
    : IQueryHandler<GetRecruitmentProfileQuery, IResult<GetRecruitmentProfileDto>>
{

    public async Task<IResult<GetRecruitmentProfileDto>> Handle(GetRecruitmentProfileQuery request, CancellationToken cancellationToken)
    {
        var userResult = await userRepository.GetByIdAsync(request.UserId);
        if (userResult.IsFailed)
            return Result.Fail<GetRecruitmentProfileDto>(userResult.Errors);
        if (userResult.Value is null)
            return Result.Fail<GetRecruitmentProfileDto>(ErrorsCodes.UserNotFound);

        var user = userResult.Value;
        return Result.Ok(new GetRecruitmentProfileDto
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