using Mapster;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummary.DTOs;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Common.Mappers.EmployeeProfiles;

public class JobInvitationSummaryProfile: IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Job, JobInvitationSummaryDto>()
            .Map(dest => dest.JobId, src => src.Id)
            .Map(dest => dest.JobName, src => src.TitleAr)
            .Map(dest => dest.JobName, src => src.TitleEn)
            .Map(dest => dest.JobStatus, src => src.JobStatus!)
            .Map(dest => dest.InvitationCount, src => src.Invitations.Count)
            .Map(dest => dest.ApplicantsCount,
                src => src.Invitations.Count(i => i.InvitationStatusId == InvitationStatusIds.Submitted))
            .Map(dest => dest.RefusedCount,
                src => src.Invitations.Count(i => i.InvitationStatusId == InvitationStatusIds.Rejected))
            .Map(dest => dest.NotSeenCount,
                src => src.Invitations.Count(i => i.InvitationStatusId == InvitationStatusIds.NewInvitation))
            .Map(dest => dest.ReadCount,
                src => src.Invitations.Count(i => i.InvitationStatusId == InvitationStatusIds.Submitted))
            .Map(dest => dest.ExpiredCount,
                src => src.Invitations.Count(i => i.InvitationStatusId == InvitationStatusIds.Closed))
            .Map(dest => dest.CancelledCount,
                src => src.Invitations.Count(i => i.InvitationStatusId == InvitationStatusIds.Cancelled))
            .Map(dest => dest.CreateDate, src => src.CreatedDate)
            .AfterMapping((src, dest) =>
            {
                var localized = MapContext.Current!.GetService<ILocalizationService>();
                dest.DepartmentName = localized.GetLocalizedName(src.Department);
                dest.JobCategory = localized.GetLocalizedName(src.JobCategory);
                var lastBatch = src.Invitations.Count == 0
                    ? 0
                    : src.Invitations.Max(invitation => (int?)invitation.BatchNumber) ?? 0;
                dest.LastBatchNumber = lastBatch;
                dest.PreviousBatchInvitations = lastBatch == 0
                    ? 0
                    : src.Invitations.Count(invitation => invitation.BatchNumber < lastBatch);
            });
    }
}
