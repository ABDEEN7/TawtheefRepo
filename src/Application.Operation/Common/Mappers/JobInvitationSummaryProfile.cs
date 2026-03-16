using Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.DTOs;
using Mapster;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Common.Mappers;

public class JobInvitationSummaryProfile: IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Job, JobInvitationSummaryDto>()
            .Map(dest => dest.JobId, src => src.Id)
            .Map(dest => dest.JobStatus, src => src.JobStatus!)
            .Map(dest => dest.InvitationCount, src => src.Invitations.Count)
            .Map(dest => dest.ApplicantsCount,
                src => src.Invitations.Count(i => i.InvitationStatusId == InvitationStatusIds.Submitted))
            .Map(dest => dest.RefusedCount,
                src => src.Invitations.Count(i => i.InvitationStatusId == InvitationStatusIds.Rejected))
            .Map(dest => dest.NotSeenCount,
                src => src.Invitations.Count(i => i.InvitationStatusId == InvitationStatusIds.NewInvitation))
            .Map(dest => dest.ReadCount,
                src => src.Invitations.Count(i => i.InvitationStatusId == InvitationStatusIds.Read))
            .Map(dest => dest.ExpiredCount,
                src => src.Invitations.Count(i => i.InvitationStatusId == InvitationStatusIds.Closed))
            .Map(dest => dest.CancelledCount,
                src => src.Invitations.Count(i => i.InvitationStatusId == InvitationStatusIds.Cancelled))
            .Map(dest => dest.CreateDate, src => src.CreatedDate)
            .AfterMapping((src, dest) =>
            {
                var localized = MapContext.Current!.GetService<ILocalizationService>();
                dest.JobName = (localized.GetCurrentLanguage() == "en" ? src.JobTitle?.JobNameEn : src.JobTitle?.JobNameAr)!;
                dest.DepartmentName = localized.GetLocalizedName(src.Department);
                dest.JobCategory = localized.GetLocalizedName(src.JobCategory);
                var lastBatch = src.Invitations
                    .OrderByDescending(invitation => invitation.CreatedDate)
                    .Select(invitation => (Guid?)invitation.BatchNumber)
                    .FirstOrDefault();
                dest.LastBatchNumber = lastBatch;
                dest.PreviousBatchInvitations = lastBatch is null
                        ? 0
                    : src.Invitations.Count(invitation => invitation.BatchNumber != lastBatch.Value);
            });
    }
}
