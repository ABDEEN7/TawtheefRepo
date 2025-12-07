using Mapster;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Recruitment.Dashboard.DTOs;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Common.Mappers.ApplicantProfiles;

public class CandidateInvitationProfile: IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Invitation, CandidateInvitationsDto>()
            .Map(dest => dest.InvitationId, src => src.Id)
            .Map(dest => dest.JobTitle, src => src.Job!.Title)
            .Map(dest => dest.InvitationStatus, src => src.InvitationStatus!)
            .AfterMapping((src, dest) =>
            {
                var localized = MapContext.Current!.GetService<ILocalizationService>();
                dest.DepartmentName = localized.GetLocalizedName(src.Job!.RequestingDepartment);
                dest.JobCategory = localized.GetLocalizedName(src.Job.JobCategory);
            });
    }
}
