using System.Text.Json;
using System.Text.RegularExpressions;
using Application.Operation.Features.Employee.MinisterOffice.Commands;
using Application.Operation.Features.Employee.MinisterOffice.DTOs;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Application.Common.Utils;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Kawader;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.MinisterOffice;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.MinisterOffice.Handlers;

public sealed class CreateMinisterOfficeCandidateCommandHandler(
    IUnitOfWork uow,
    IMoiClient moiClient,
    IAppLogger logger)
    : IRequestHandler<CreateMinisterOfficeCandidateCommand, IResult<MinisterOfficeCandidateDto>>
{
    private static readonly Regex PhoneRegex = new(@"^\+974\d{8}$", RegexOptions.Compiled);

    public async Task<IResult<MinisterOfficeCandidateDto>> Handle(
        CreateMinisterOfficeCandidateCommand request,
        CancellationToken ct)
    {
        var req = request.Request;

        // ── 1. VALIDATE INPUT ──────────────────────────────────
        var normalizedQid = QidUtilities.Normalize(req.Qid);
        if (!QidUtilities.IsValid(normalizedQid))
            return Result.Fail<MinisterOfficeCandidateDto>(ErrorsCodes.InvalidQidFormat);

        var phone = MoiUtils.NormalizePhone(req.PhoneNumber);
        if (!PhoneRegex.IsMatch(phone))
            return Result.Fail<MinisterOfficeCandidateDto>(ErrorsCodes.InvalidPhoneFormat);

        if (req.QidExpiryDate == default)
            return Result.Fail<MinisterOfficeCandidateDto>(ErrorsCodes.QidExpiryDateRequired);

        // ── 2. CHECK EXISTING CANDIDATE ────────────────────────
        var candidateRepo = uow.GetEntityRepository<MinisterOfficeCandidate>();
        var existing = await candidateRepo.DbSet
            .FirstOrDefaultAsync(c => c.Qid == normalizedQid, ct);

        if (existing is not null)
        {
            var auditAction = MinisterOfficeCandidateAuditActions.Linked;
            var details = "Linked existing candidate to Minister Office request";

            if (!existing.IsFollowUpActive)
            {
                existing.IsFollowUpActive = true;
                auditAction = MinisterOfficeCandidateAuditActions.FollowUpReactivated;
                details = "Re-activated follow-up for existing candidate on registration attempt";
            }

            if (existing.PhoneNumber != phone)
            {
                existing.PhoneNumber = phone;
                details += " (also updated phone number)";
            }

            // Write audit and save
            await WriteAuditLogAsync(existing.Id, normalizedQid, auditAction, details);
            await uow.SaveChangesAsync(ct);

            var status = await ComputeStatusAsync(normalizedQid, ct);
            return Result.Ok(MapToDto(existing, status));
        }

        // ── 3. CALL MOI API ────────────────────────────────────
        var moiResult = await moiClient.GetPersonalInfoAsync(normalizedQid, req.QidExpiryDate, ct);
        if (moiResult.IsFailed)
        {
            logger.Warning("MOI validation failed for QID {Qid}: {Errors}",
                MoiUtils.MaskQid(normalizedQid),
                string.Join("; ", moiResult.Errors.Select(e => e.Message)));

            return Result.Fail<MinisterOfficeCandidateDto>(ErrorsCodes.MoiValidationFailed);
        }

        var moi = moiResult.Value;

        // ── 4. KAWADER CHECK (Qatari only) ─────────────────────
        var isKawaderUser = false;
        if (moi.NationalityCode == MoiUtils.QatarNationalityCode)
        {
            var kawaderRepo = uow.GetEntityRepository<KawaderQid>();
            var kawaderExists = await kawaderRepo.DbSet
                .AnyAsync(k => k.Qid == normalizedQid, ct);

            if (!kawaderExists)
                return Result.Fail<MinisterOfficeCandidateDto>(ErrorsCodes.KawaderRegistrationRequired);

            isKawaderUser = true;
        }

        // ── 5. PERSIST CANDIDATE ───────────────────────────────
        var candidate = new MinisterOfficeCandidate
        {
            Qid = normalizedQid,
            PhoneNumber = phone,
            QidExpiryDate = req.QidExpiryDate,
            FullNameEn = moi.EnglishFullName,
            FullNameAr = moi.ArabicFullName,
            NationalityEn = moi.NationalityNameEnglish,
            NationalityAr = moi.NationalityNameArabic,
            NationalityCode = moi.NationalityCode,
            IsFollowUpActive = true
        };

        await candidateRepo.AddAsync(candidate, ct);

        // ── 6. SEND SMS (skip for Kawader users) ───────────────
        if (!isKawaderUser)
        {
            var smsBody = $"Dear {moi.EnglishFullName}, you have been registered in the Minister Office Recruitment System.";

            var notification = Notification.Create(
                NotificationChannel.Sms,
                "MinisterOfficeRegistration",
                null,
                phone,
                "Minister Office Registration",
                smsBody,
                smsBody,
                JsonSerializer.Serialize(new { Qid = MoiUtils.MaskQid(normalizedQid) }),
                $"minister-office-reg-{normalizedQid}");

            var notifRepo = uow.GetEntityRepository<Notification>();
            await notifRepo.AddAsync(notification, ct);
        }

        // ── 7. AUDIT LOG ───────────────────────────────────────
        await WriteAuditLogAsync(candidate.Id, normalizedQid,
            MinisterOfficeCandidateAuditActions.Created,
            $"Created with MOI data. Kawader={isKawaderUser}. Nationality={moi.NationalityNameEnglish}");

        // ── 8. SAVE CHANGES ────────────────────────────────────
        await uow.SaveChangesAsync(ct);

        // ── 9. COMPUTE STATUS + RETURN ─────────────────────────
        var computedStatus = await ComputeStatusAsync(normalizedQid, ct);
        return Result.Ok(MapToDto(candidate, computedStatus));
    }

    // ─────────────────────────────────────────────────────────
    // Status computation: always by QID lookup into UserProfile
    // ─────────────────────────────────────────────────────────
    private async Task<MinisterOfficeCandidateStatus> ComputeStatusAsync(string qid, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var profile = await profileRepo.DbSet
            .AsNoTracking()
            .Where(p => p.NationalNumber == qid)
            .OrderByDescending(p => p.CreatedDate)
            .FirstOrDefaultAsync(ct);

        if (profile is null)
            return MinisterOfficeCandidateStatus.NoProfile;

        // Check for invitations if profile is approved
        if (profile.Status == UserProfileStatus.Approved)
        {
            var invRepo = uow.GetEntityRepository<Invitation>();
            var hasInvitations = await invRepo.DbSet
                .AnyAsync(i => i.ApplicantId == profile.UserId, ct);

            return hasInvitations
                ? MinisterOfficeCandidateStatus.InvitationsReceived
                : MinisterOfficeCandidateStatus.ApprovedProfile;
        }

        return profile.Status switch
        {
            UserProfileStatus.InCreation => MinisterOfficeCandidateStatus.DraftProfile,
            UserProfileStatus.Submitted => MinisterOfficeCandidateStatus.SubmittedForApproval,
            UserProfileStatus.UnderReview => MinisterOfficeCandidateStatus.SubmittedForApproval,
            UserProfileStatus.RequiresUpdate => MinisterOfficeCandidateStatus.ReturnedForCorrection,
            _ => MinisterOfficeCandidateStatus.NoProfile
        };
    }

    private async Task WriteAuditLogAsync(Guid candidateId, string qid, string action, string? details)
    {
        var auditRepo = uow.GetEntityRepository<MinisterOfficeCandidateAuditLog>();
        await auditRepo.AddAsync(new MinisterOfficeCandidateAuditLog
        {
            CandidateId = candidateId,
            Qid = qid,
            Action = action,
            Details = details
        });
    }

    private static MinisterOfficeCandidateDto MapToDto(
        MinisterOfficeCandidate entity,
        MinisterOfficeCandidateStatus status) => new()
    {
        Id = entity.Id,
        Qid = entity.Qid,
        FullNameEn = entity.FullNameEn,
        FullNameAr = entity.FullNameAr,
        PhoneNumber = entity.PhoneNumber,
        NationalityEn = entity.NationalityEn,
        NationalityAr = entity.NationalityAr,
        IsFollowUpActive = entity.IsFollowUpActive,
        Status = status,
        CreatedDate = entity.CreatedDate
    };
}
