using Application.Recruitment.Features.Profile.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Queries;

public sealed record GetMyProfileChangeRequestsQuery(Guid UserId) : IQuery<Result<IReadOnlyList<ProfileChangeRequestDto>>>;

public sealed class GetMyProfileChangeRequestsHandler(IUnitOfWork uow) 
    : IQueryHandler<GetMyProfileChangeRequestsQuery, Result<IReadOnlyList<ProfileChangeRequestDto>>>
{
    public async Task<Result<IReadOnlyList<ProfileChangeRequestDto>>> Handle(GetMyProfileChangeRequestsQuery request, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();

        var profile = await profileRepo.DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, ct);

        if (profile is null)
            return Result.Fail<IReadOnlyList<ProfileChangeRequestDto>>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status != UserProfileStatus.Approved)
            return Result.Ok<IReadOnlyList<ProfileChangeRequestDto>>([]);

        var changeRepo = uow.GetEntityRepository<ProfileChangeRequest>();
        var items = await changeRepo.DbSet
            .AsNoTracking()
            .Where(c => c.UserProfileId == profile.Id && c.Status != ProfileChangeRequestStatus.Canceled)
            .OrderByDescending(c => c.RequestedAtUtc)
            .Select(c => new ProfileChangeRequestDto
            {
                Id = c.Id,
                Section = c.Section,
                Action = c.Action,
                Status = c.Status,
                TargetKey = c.TargetKey,
                FieldPath = c.FieldPath,
                EntityName = c.EntityName,
                OldValue = c.OldValue,
                NewValue = c.NewValue,
                RequestedAtUtc = c.RequestedAtUtc,
                ReviewedAtUtc = c.ReviewedAtUtc,
                ReviewerNote = c.ReviewerNote
            })
            .ToListAsync(ct);

        return Result.Ok<IReadOnlyList<ProfileChangeRequestDto>>(items);
    }
}
