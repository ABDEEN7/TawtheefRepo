using Application.Operation.Features.Employee.ProfileManagement;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs;
using Application.Operation.Features.Employee.CandidateUsers.Queries;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.ProfileApproval;
using FluentResults;
using Mapster;
using MapsterMapper;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Application.Common.Mappers;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.CandidateUsers.Handlers.Queries;

public class GetCandidateUserProfileHandler(IUnitOfWork uow, IMapper mapper, 
    IMediaUrlResolver media, ILocalizationService localization)
    : IRequestHandler<GetCandidateUserProfileQuery, Result<GetProfileApprovalDetailDto>>
{
    public async Task<Result<GetProfileApprovalDetailDto>> Handle(GetCandidateUserProfileQuery request, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, request.UserId, ct: ct);
        if (profile is null)
            return Result.Fail<GetProfileApprovalDetailDto>(ErrorsCodes.UserProfileNotFound);

        var profileData = MapProfile(profile);
        
        var dto = new GetProfileApprovalDetailDto
        {
            UserProfileId = profile.Id,
            UserId = profile.UserId,
            FullName = localization.GetLocalizedFullName(profile.User),
            CandidateType = localization.GetLocalizedName(profile.CandidateType),
            TargetEntity = localization.GetLocalizedName(profile.TargetEntity),
            Profile = profileData,
            ProfileStatus = (int)profile.Status
        };

        return Result.Ok(dto);
    }

    private ProfileApprovalDataDto MapProfile(UserProfile profileEntity)
    {
        using var scope = new MapContextScope();
        scope.Context.Parameters[ResourceMapper.MediaKey] = media;
        return mapper.Map<ProfileApprovalDataDto>(profileEntity);
    }
}
