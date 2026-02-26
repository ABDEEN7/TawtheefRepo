using Application.Operation.Features.Admin.ProfileLogs;
using Application.Operation.Features.Admin.SystemAdminLogs.DTOs;
using Application.Operation.Features.Admin.SystemAdminLogs.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Admin.SystemAdminLogs.Handlers.Queries;

public sealed class GetSystemAdminLogsQueryHandler(
    IUnitOfWork uow,
    UserManager<User> userManager)
    : IQueryHandler<GetSystemAdminLogsQuery, IResult<PaginatedResult<SystemAdminLogDto>>>
{
    public async Task<IResult<PaginatedResult<SystemAdminLogDto>>> Handle(
        GetSystemAdminLogsQuery request,
        CancellationToken cancellationToken)
    {
        var auditRepo = uow.GetEntityRepository<AuditTrailEntry>();

        var query = auditRepo.DbSet
            .AsNoTracking()
            .WhereIf(request.UserProfileId.HasValue, log => log.UserProfileId == request.UserProfileId!.Value)
            .WhereIf(request.UserId.HasValue, log => log.UserId == request.UserId!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(request.ActionType), log => log.ActionType == request.ActionType)
            .WhereIf(request.From.HasValue, log => log.CreatedDate >= request.From!.Value)
            .WhereIf(request.To.HasValue, log => log.CreatedDate <= request.To!.Value)
            .Select(log => new SystemAdminLogProjection
            {
                Id = log.Id,
                UserProfileId = log.UserProfileId,
                UserId = log.UserId,
                ActionType = log.ActionType,
                Section = log.Section,
                Notes = log.Notes,
                EntityId = log.EntityId,
                AttachmentId = log.AttachmentId,
                //ReviewStatus = null,
                CreatedDate = log.CreatedDate
            });

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
            .Select(log => new SystemAdminLogDto
            {
                Id = log.Id,
                UserProfileId = log.UserProfileId,
                UserId = log.UserId,
                UserName = userLookup.GetValueOrDefault(log.UserId),
                Source = ProfileLogSources.AuditTrail,
                ActionType = log.ActionType,
                Section = log.Section,
                Notes = log.Notes,
                EntityId = log.EntityId,
                AttachmentId = log.AttachmentId,
                ReviewStatus = null,
                CreatedDate = log.CreatedDate
            })
            .ToList();

        return Result.Ok(new PaginatedResult<SystemAdminLogDto>(mapped, totalCount, request.PageNumber, request.PageSize));
    }

    private async Task<Dictionary<Guid, string>> BuildUserLookupAsync(
        IEnumerable<SystemAdminLogProjection> logs,
        CancellationToken cancellationToken)
    {
        var userIds = logs
            .Select(l => l.UserId)
            .Distinct()
            .ToArray();

        if (userIds.Length == 0)
            return new Dictionary<Guid, string>();

        var users = await userManager.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FullNameAr, u.FullNameEn, u.Email })
            .ToListAsync(cancellationToken);

        return users.ToDictionary(
            u => u.Id,
            u => !string.IsNullOrWhiteSpace(u.FullNameAr)
                ? u.FullNameAr
                : !string.IsNullOrWhiteSpace(u.FullNameEn)
                    ? u.FullNameEn
                    : u.Email ?? string.Empty);
    }

    private sealed record SystemAdminLogProjection
    {
        public required Guid Id { get; init; }
        public required Guid UserProfileId { get; init; }
        public required Guid UserId { get; init; }
        public required string ActionType { get; init; }
        public string? Section { get; init; }
        public string? Notes { get; init; }
        public Guid? EntityId { get; init; }
        public Guid? AttachmentId { get; init; }
        public required DateTimeOffset CreatedDate { get; init; }
    }
}
