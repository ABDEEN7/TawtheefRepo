using Mapster;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.DTOs;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Common.Mappers.EmployeeProfiles;

public sealed class JobInvitationSummaryDetailsProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Job, JobInvitationSummaryDetailsInfoDto>()
            .Map(dest => dest.JobId, src => src.Id)
            .Map(dest => dest.JobName, src => src.TitleEn);

        config.NewConfig<Invitation, JobInvitationSummaryDetailsRowDto>()
            .Map(dest => dest.InviteId, src => src.Id)
            .Map(dest => dest.Phone, src => src.Applicant!.PhoneNumber ?? string.Empty)
            .Map(dest => dest.SentDate, src => src.CreatedDate)
            .Ignore(dest => dest.Status)
            .Ignore(dest => dest.FullName)
            .Ignore(dest => dest.Nationality);
    }
}
