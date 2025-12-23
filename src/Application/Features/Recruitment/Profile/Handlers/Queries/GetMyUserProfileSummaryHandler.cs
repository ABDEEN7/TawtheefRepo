using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Application.Features.Recruitment.Profile.Queries;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Queries;

public sealed class GetMyUserProfileSummaryHandler(IUnitOfWork uow, UserManager<User> userManager)
    : IRequestHandler<GetMyUserProfileSummaryQuery, Result<UserProfileSummaryDto>>
{
    public async Task<Result<UserProfileSummaryDto>> Handle(GetMyUserProfileSummaryQuery request, CancellationToken ct)
    {
        var profileResult = await UserProfileLoader.GetOrCreateAsync(uow, userManager, request.UserId, ct);
        if (profileResult.IsFailed) return Result.Fail<UserProfileSummaryDto>(profileResult.Errors);

        var profile = profileResult.Value;
        var canEdit = profile.Status == UserProfileStatus.InCreation;
        var canSubmit = canEdit && profile.IsCompleted();

        return Result.Ok(new UserProfileSummaryDto
        {
            UserProfileId = profile.Id,
            Status = profile.Status,
            CanEdit = canEdit,
            CanSubmit = canSubmit
        });
    }
}
