using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command;

public sealed class GetMyProfileCorrectionsHandler(IUnitOfWork uow)
    : IRequestHandler<GetMyProfileCorrectionsQuery, Result<ProfileCorrectionsDto>>
{
    public async Task<Result<ProfileCorrectionsDto>> Handle(GetMyProfileCorrectionsQuery request, CancellationToken ct)
    {
        var profile = await uow.GetEntityRepository<UserProfile>().DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, ct);

        if (profile is null)
            return Result.Fail<ProfileCorrectionsDto>(ErrorsCodes.UserProfileNotFound);

        // Critical: no notes leakage before finalize
        if (profile.Status != UserProfileStatus.InCreation)
        {
            return Result.Ok(new ProfileCorrectionsDto
            {
                Status = profile.Status,
                Items = []
            });
        }

        var items = await uow.GetEntityRepository<ReviewItem>().DbSet
            .AsNoTracking()
            .Where(r => r.UserProfileId == profile.Id &&
                        r.TargetType == ReviewTargetType.Section &&
                        r.Status == ReviewStatus.NeedsCorrection)
            .Select(r => new CorrectionItemDto
            {
                Section = r.Section,
                Note = r.ReviewerNote ?? string.Empty
            })
            .OrderBy(x => (int)x.Section)
            .ToListAsync(ct);

        return Result.Ok(new ProfileCorrectionsDto
        {
            Status = profile.Status,
            Items = items
        });
    }
}
