using Mapster;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.DTOs;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Common.Mappers.EmployeeProfiles;

public sealed class JobInvitationSummaryDetailsProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Job, JobInvitationSummaryDetailsInfoDto>()
            .Map(dest => dest.JobId, src => src.Id)
            .Map(dest => dest.JobName, src => src.TitleEn)
            .AfterMapping((src, dest) =>
            {
                var localized = MapContext.Current?.GetService<ILocalizationService>();
                dest.JobName = localized?.GetLocalizedValue(src.TitleAr, src.TitleEn)  ?? src.TitleEn;
            });

        config.NewConfig<Invitation, JobInvitationSummaryDetailsRowDto>()
            .Map(dest => dest.InviteId, src => src.Id)
            .Map(dest => dest.Phone, src => src.Applicant!.PhoneNumber ?? string.Empty)
            .Map(dest => dest.SentDate, src => src.CreatedDate)
            .AfterMapping((src, dest) =>
            {
                var localized = MapContext.Current!.GetService<ILocalizationService>();
                dest.FullName = localized.GetLocalizedFullName(src.Applicant);
                dest.Nationality = localized.GetLocalizedName(src.Applicant?.Profile?.Nationality);
            });
    }
}
