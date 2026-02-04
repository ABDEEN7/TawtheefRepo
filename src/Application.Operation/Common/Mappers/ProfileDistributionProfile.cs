using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;
using Mapster;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Common.Mappers;

public sealed class ProfileDistributionProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserProfile, DistributionProfileDto>()
            .Map(dest => dest.ProfileId, src => src.Id)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.SubmittedAtUtc, src => src.CreatedDate)
            .Ignore(dest => dest.AssignedEmployeeId)
            .Ignore(dest => dest.AssignedEmployeeName)
            .AfterMapping((src, dest) =>
            {
                var localized = MapContext.Current!.GetService<ILocalizationService>();
                dest.CandidateName = localized.GetLocalizedFullName(src.User);
                dest.Specialization = localized.GetLocalizedName(src.CandidateType);
                dest.TargetEntity = localized.GetLocalizedName(src.TargetEntity);
            });
    }
}
