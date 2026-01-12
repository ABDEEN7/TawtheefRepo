using Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.DTOs;
using Mapster;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Common.Mappers;

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
            .Map(dest => dest.BatchNumber, src => src.BatchNumber)
            .Map(dest => dest.SentDate, src => src.CreatedDate)
            .Map(dest => dest.AppliedDate, src => src.AcceptedAt)
            .Ignore(dest => dest.Status)
            .Ignore(dest => dest.FullName)
            .Ignore(dest => dest.Nationality)
            .Ignore(dest => dest.PersonalNumber)
            .Ignore(dest => dest.ReadDate!)
            .Ignore(dest => dest.DeclinedDate!)
            .Ignore(dest => dest.ExpiredDate!);
    }
}
