using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.DTOs;
using Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.Handlers.Queries;

public class GetProfileApprovalDetailHandler(IUnitOfWork uow, IMediaUrlResolver media)
    : IRequestHandler<GetProfileApprovalDetailQuery, Result<ProfileApprovalDetailDto>>
{
    public async Task<Result<ProfileApprovalDetailDto>> Handle(GetProfileApprovalDetailQuery request, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();

        var profile = await profileRepo.DbSet
            .Include(p => p.User)
            .Include(p => p.CandidateType)
            .Include(p => p.TargetEntity)
            .FirstOrDefaultAsync(p => p.Id == request.UserProfileId, ct);

        if (profile is null)
            return Result.Fail<ProfileApprovalDetailDto>(ErrorsCodes.UserProfileNotFound);

        var submissionRepo = uow.GetEntityRepository<ProfileSubmission>();
        var submission = await submissionRepo.DbSet
            .Where(s => s.UserProfileId == profile.Id)
            .OrderByDescending(s => s.Version)
            .FirstOrDefaultAsync(ct);

        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        var reviewItems = await reviewRepo.DbSet
            .Where(r => r.UserProfileId == profile.Id)
            .OrderByDescending(r => r.Version)
            .ToListAsync(ct);

        var latestItems = reviewItems
            .GroupBy(r => new { r.TargetType, r.Section, r.FieldPath, r.EntityName, r.EntityId, r.ResourceId })
            .Select(g => g.First())
            .ToList();

        var resourceIds = latestItems
            .Where(r => r.ResourceId.HasValue)
            .Select(r => r.ResourceId!.Value)
            .Distinct()
            .ToList();

        var resourceRepo = uow.GetEntityRepository<Resource>();
        var resources = await resourceRepo.DbSet
            .Where(r => resourceIds.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id, ct);

        var sections = latestItems
            .GroupBy(r => r.Section)
            .Select(group =>
            {
                var sectionReview = group.FirstOrDefault(i => i.TargetType == ReviewTargetType.Section);
                var entries = group
                    .Where(i => i.TargetType != ReviewTargetType.Section)
                    .Select(MapItem)
                    .ToList();

                return new ProfileApprovalSectionDto
                {
                    Section = group.Key,
                    SectionReview = sectionReview is null ? null : MapItem(sectionReview),
                    Items = entries,
                    HasAttachments = entries.Any(e => e.TargetType == ReviewTargetType.Attachment)
                };
            })
            .OrderBy(s => (int)s.Section)
            .ToList();

        var dto = new ProfileApprovalDetailDto
        {
            UserProfileId = profile.Id,
            UserId = profile.UserId,
            FullName = profile.User?.FullNameEn ?? profile.User?.FullNameAr ?? string.Empty,
            CandidateType = profile.CandidateType?.NameAr ?? profile.CandidateType?.NameEn,
            TargetEntity = profile.TargetEntity?.NameAr ?? profile.TargetEntity?.NameEn,
            SubmissionVersion = submission?.Version,
            SubmittedAtUtc = submission?.SubmittedAtUtc,
            Sections = sections
        };

        return Result.Ok(dto);

        ProfileApprovalItemDto MapItem(ReviewItem item)
        {
            var title = item.TargetType switch
            {
                ReviewTargetType.Section => "Textual data",
                ReviewTargetType.Attachment => item.AttachmentTitle ?? "Attachment",
                ReviewTargetType.Row => item.EntityName ?? "Row",
                ReviewTargetType.Field => item.FieldPath ?? "Field",
                _ => "Review item"
            };

            string? resourceUrl = null;
            if (item.ResourceId.HasValue && resources.TryGetValue(item.ResourceId.Value, out var resource))
            {
                resourceUrl = media.ResolveAbsolute(resource.Url);
            }

            return new ProfileApprovalItemDto
            {
                ReviewItemId = item.Id,
                TargetType = item.TargetType,
                Status = item.Status,
                Title = title,
                Note = item.ReviewerNote,
                ResourceId = item.ResourceId,
                ResourceUrl = resourceUrl,
                EntityId = item.EntityId,
                EntityName = item.EntityName,
                Version = item.Version,
                ApprovedAtVersion = item.ApprovedAtVersion,
                ReviewedAtUtc = item.ReviewedAtUtc
            };
        }
    }
}
