using Application.Recruitment.Features.Profile.DTOs;
using Application.Recruitment.Features.Profile.Queries;
using MediatR;
using FluentResults;
using Mapster;
using MapsterMapper;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Application.Common.Mappers;
using Tawtheef.Domain.Constants;

namespace Application.Recruitment.Features.Profile.Handlers.Queries;

public sealed class GetMyUserProfileHandler(IUnitOfWork uow, IMapper mapper, IMediaUrlResolver media)
    : IRequestHandler<GetMyUserProfileQuery, Result<UserProfileViewDto>>
{
    public async Task<Result<UserProfileViewDto>> Handle(GetMyUserProfileQuery request, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, request.UserId, ct: ct);

        if (profile is null)
            return Result.Fail<UserProfileViewDto>(ErrorsCodes.UserProfileNotFound);

        using var scope = new MapContextScope();
        scope.Context.Parameters[ResourceMapper.MediaKey] = media;
        var snapshot = mapper.Map<ProfileApprovalDataDto>(profile);

        return Result.Ok(new UserProfileViewDto
        {
            UserProfileId = profile.Id,
            Status = profile.Status,
            Profile = snapshot
        });
    }
}

