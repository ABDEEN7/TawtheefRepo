using Application.Recruitment.Features.Profile.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Application.Common.Mappers;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Queries;

public sealed class GetMyProfileStatusHandler(IUnitOfWork uow, UserManager<User> userManager, 
    IMapper mapper, IMediaUrlResolver media)
    : IQueryHandler<GetMyProfileStatusQuery, Result<ProfileStatusDto>>
{
    public async Task<Result<ProfileStatusDto>> Handle(GetMyProfileStatusQuery request, CancellationToken ct)
    {
        var query = BuildSectionQuery(request.Section);

        var profile = await query.FirstOrDefaultAsync(p => p.UserId == request.UserId, ct);
        if (profile is null)
        {
            var user = await userManager.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == request.UserId, ct);

            return Result.Ok(new ProfileStatusDto
            {
                AgreedToTerms = user?.AgreedToTerms ?? false
            });
        }

        using var scope = new MapContextScope();
        scope.Context.Parameters[ResourceMapper.MediaKey] = media;

        var source = new ProfileBootstrapSource(profile, profile.User!, new ProfilePrefillDto());
        var dto = mapper.Map<ProfileStatusDto>(source);
        return Result.Ok(dto);
    }

    private IQueryable<UserProfile> BuildSectionQuery(ProfileSection? section)
        => UserProfileQueryFactory.CreateSectionQuery(uow, section);
}
