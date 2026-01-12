using Application.Operation.Features.Admin.ProfileLogs.DTOs;
using Application.Operation.Features.Admin.ProfileLogs.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Admin.ProfileLogs.Handlers.Queries;

public sealed class GetProfileLogsQueryHandler(
    IUnitOfWork uow,
    UserManager<User> userManager)
    : IQueryHandler<GetProfileLogsQuery, IResult<PaginatedResult<ProfileLogDto>>>
{
    public async Task<IResult<PaginatedResult<ProfileLogDto>>> Handle(
        GetProfileLogsQuery request,
        CancellationToken cancellationToken)
    {
        var loggerRepo = uow.GetEntityRepository<UserProfileLogger>();
        var auditRepo = uow.GetEntityRepository<AuditTrailEntry>();

        IQueryable<ProfileLogProjection> query = loggerRepo.DbSet
            .AsNoTracking()
            .Select(log => new ProfileLogProjection
            {
                Id = log.Id,
                UserProfileId = log.UserProfileId,
                UserId = log.PerformedById ?? log.CreatedById,
                ActionType = log.ActionType,
                Section = log.Section,
                Notes = log.Notes,
                ReviewStatus = log.ReviewStatus,
                EntityId = log.EntityId,
                AttachmentId = log.AttachmentId,
                Source = ProfileLogSources.UserProfileLogger,
                CreatedDate = log.CreatedDate
            })
            .Concat(auditRepo.DbSet
                .AsNoTracking()
                .Select(log => new ProfileLogProjection
                {
                    Id = log.Id,
                    UserProfileId = log.UserProfileId,
                    UserId = log.UserId,
                    ActionType = log.ActionType,
                    Section = log.Section,
                    Notes = log.Notes,
                    ReviewStatus = null,
                    EntityId = log.EntityId,
                    AttachmentId = log.AttachmentId,
                    Source = ProfileLogSources.AuditTrail,
                    CreatedDate = log.CreatedDate
                }));

        query = query
            .WhereIf(request.UserProfileId.HasValue, log => log.UserProfileId == request.UserProfileId!.Value)
            .WhereIf(request.UserId.HasValue, log => log.UserId == request.UserId!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Source), log => log.Source == request.Source)
            .WhereIf(!string.IsNullOrWhiteSpace(request.ActionType), log => log.ActionType == request.ActionType)
            .WhereIf(request.ReviewStatus.HasValue, log => log.ReviewStatus == request.ReviewStatus)
            .WhereIf(request.From.HasValue, log => log.CreatedDate >= request.From!.Value)
            .WhereIf(request.To.HasValue, log => log.CreatedDate <= request.To!.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(log =>
                EF.Functions.Like(log.ActionType, $"%{search}%") ||
                (log.Section != null && EF.Functions.Like(log.Section, $"%{search}%")) ||
                (log.Notes != null && EF.Functions.Like(log.Notes, $"%{search}%")));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(log => log.CreatedDate)
            .ThenByDescending(log => log.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var userLookup = await BuildUserLookupAsync(items, cancellationToken);

        var mapped = items
            .Select(log => new ProfileLogDto
            {
                Id = log.Id,
                UserProfileId = log.UserProfileId,
                Source = log.Source,
                ActionType = log.ActionType,
                Section = log.Section,
                Notes = log.Notes,
                UserId = log.UserId,
                UserName = log.UserId.HasValue && userLookup.TryGetValue(log.UserId.Value, out var name)
                    ? name
                    : null,
                EntityId = log.EntityId,
                AttachmentId = log.AttachmentId,
                ReviewStatus = log.ReviewStatus,
                CreatedDate = log.CreatedDate
            })
            .ToList();

        var result = new PaginatedResult<ProfileLogDto>(mapped, totalCount, request.PageNumber, request.PageSize);

        return Result.Ok(result);
    }

    private async Task<Dictionary<Guid, string>> BuildUserLookupAsync(
        IEnumerable<ProfileLogProjection> logs,
        CancellationToken cancellationToken)
    {
        var userIds = logs
            .Select(l => l.UserId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToArray();

        if (userIds.Length == 0)
        {
            return new Dictionary<Guid, string>();
        }

        var users = await userManager.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new
            {
                u.Id,
                u.FullNameAr,
                u.FullNameEn,
                u.Email
            })
            .ToListAsync(cancellationToken);

        return users.ToDictionary(
            u => u.Id,
            u => !string.IsNullOrWhiteSpace(u.FullNameAr)
                ? u.FullNameAr
                : !string.IsNullOrWhiteSpace(u.FullNameEn)
                    ? u.FullNameEn
                    : u.Email ?? string.Empty);
    }

    private sealed record ProfileLogProjection
    {
        public required Guid Id { get; init; }
        public required Guid UserProfileId { get; init; }
        public Guid? UserId { get; init; }
        public required string ActionType { get; init; }
        public string? Section { get; init; }
        public string? Notes { get; init; }
        public ReviewStatus? ReviewStatus { get; init; }
        public Guid? EntityId { get; init; }
        public Guid? AttachmentId { get; init; }
        public required string Source { get; init; }
        public required DateTimeOffset CreatedDate { get; init; }
    }
}
