using FluentResults;
using Mapster;
using MapsterMapper;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Mappers;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.DTOs;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Application.Features.Recruitment.Profile.Queries;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Queries;

public sealed class GetMyUserProfileHandler(IUnitOfWork uow, IMapper mapper, IMediaUrlResolver media)
    : IRequestHandler<GetMyUserProfileQuery, Result<UserProfileViewDto>>
{
    public async Task<Result<UserProfileViewDto>> Handle(GetMyUserProfileQuery request, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfile(uow, request.UserId, ct: ct);

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
