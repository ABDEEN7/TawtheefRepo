using Application.Operation.Features.Employee.QuestionBanks.DTOs;
using Mapster;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Application.Operation.Features.Employee.QuestionBanks.Mappers;

public sealed class QuestionBankProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<QuestionBank, QuestionBankListItemDto>()
            .Map(dest => dest.QuestionBankTypeNameAr, src => src.QuestionBankType.NameAr)
            .Map(dest => dest.QuestionBankTypeNameEn, src => src.QuestionBankType.NameEn)
            .Map(dest => dest.ManagementNameAr,
                src => src.Management == null ? null : src.Management.NameAr)
            .Map(dest => dest.ManagementNameEn,
                src => src.Management == null ? null : src.Management.NameEn)
            .Map(dest => dest.JobTitleNameAr,
                src => src.JobTitle == null ? null : src.JobTitle.JobNameAr)
            .Map(dest => dest.JobTitleNameEn,
                src => src.JobTitle == null ? null : src.JobTitle.JobNameEn)
            .Map(dest => dest.StageNameAr, src => src.Stage == null ? null : src.Stage.NameAr)
            .Map(dest => dest.StageNameEn, src => src.Stage == null ? null : src.Stage.NameEn)
            .Map(dest => dest.CurrentVersionNo,
                src => src.CurrentApprovedVersion == null
                    ? (int?)null
                    : src.CurrentApprovedVersion.VersionNo);
    }
}
