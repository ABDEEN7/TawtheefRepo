using Application.Operation.Features.Employee.MinisterOffice.DTOs;
using Application.Operation.Features.Employee.MinisterOffice.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.MinisterOffice;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.MinisterOffice.Handlers;

public sealed class GetMinisterOfficeCandidateInvitationsQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetMinisterOfficeCandidateInvitationsQuery,
        IResult<IReadOnlyList<MinisterOfficeCandidateInvitationDto>>>
{
    public async Task<IResult<IReadOnlyList<MinisterOfficeCandidateInvitationDto>>> Handle(
        GetMinisterOfficeCandidateInvitationsQuery request,
        CancellationToken ct)
    {
        // Load candidate
        var candidateRepo = uow.GetEntityRepository<MinisterOfficeCandidate>();
        var candidate = await candidateRepo.DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CandidateId, ct);

        if (candidate is null)
            return Result.Fail<IReadOnlyList<MinisterOfficeCandidateInvitationDto>>(
                ErrorsCodes.MinisterOfficeCandidateNotFound);

        // Find user profile by QID
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var profile = await profileRepo.DbSet
            .AsNoTracking()
            .Where(p => p.NationalNumber == candidate.Qid)
            .OrderByDescending(p => p.CreatedDate)
            .FirstOrDefaultAsync(ct);

        if (profile is null)
            return Result.Ok<IReadOnlyList<MinisterOfficeCandidateInvitationDto>>(
                []);

        // Query invitations with job info
        var invRepo = uow.GetEntityRepository<Invitation>();
        var invitations = await invRepo.DbSet
            .AsNoTracking()
            .Include(i => i.Job)
            .Where(i => i.ApplicantId == profile.UserId)
            .OrderByDescending(i => i.CreatedDate)
            .Select(i => new MinisterOfficeCandidateInvitationDto
            {
                InvitationId = i.Id,
                JobTitleEn = i.Job != null ? i.Job.JobTitle!.JobNameEn : string.Empty,
                JobTitleAr = i.Job != null ? i.Job.JobTitle!.JobNameAr : string.Empty,
                InvitationStatus = MapInvitationStatus(i.InvitationStatusId),
                InvitedAt = i.CreatedDate
            })
            .ToListAsync(ct);

        return Result.Ok<IReadOnlyList<MinisterOfficeCandidateInvitationDto>>(invitations);
    }

    private static string MapInvitationStatus(Guid statusId)
    {
        if (statusId == InvitationStatusIds.NewInvitation ||
            statusId == InvitationStatusIds.Read)
            return "Pending";

        if (statusId == InvitationStatusIds.Submitted ||
            statusId == InvitationStatusIds.PendingAttachmentApproval)
            return "Accepted";

        if (statusId == InvitationStatusIds.Rejected ||
            statusId == InvitationStatusIds.Cancelled)
            return "Rejected";

        if (statusId == InvitationStatusIds.Closed)
            return "Closed";

        return "Unknown";
    }
}
