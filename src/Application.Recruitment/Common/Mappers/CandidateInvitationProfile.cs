using Application.Recruitment.Features.Dashboard.DTOs;
using Mapster;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Recruitment.Common.Mappers;

public class CandidateInvitationProfile: IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Invitation, CandidateInvitationsDto>()
            .Map(dest => dest.InvitationId, src => src.Id)
            .Map(dest => dest.InvitationStatus, src => src.InvitationStatus!)
            .Map(dest => dest.DepartmentName,src => src.Job!.Department)
            .Map(dest => dest.JobCategory,src => src.Job!.JobCategory)
            .Map(dest => dest.JobCategoryBackendName,src => src.Job!.JobCategory!.BackendName)
            .AfterMapping((src, dest) =>
            {
                var localized = MapContext.Current!.GetService<ILocalizationService>();
                dest.JobTitle = localized.GetLocalizedValue(src.Job!.TitleAr, src.Job!.TitleEn);
            });
    }
}
