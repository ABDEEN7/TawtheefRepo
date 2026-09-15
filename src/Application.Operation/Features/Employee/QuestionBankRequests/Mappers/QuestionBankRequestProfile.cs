using Application.Operation.Features.Employee.QuestionBankRequests.DTOs;
using Mapster;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Application.Operation.Features.Employee.QuestionBankRequests.Mappers;

public sealed class QuestionBankRequestProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<QuestionBankRequest, QuestionBankRequestListItemDto>()
            .Map(d => d.RequestTypeNameAr, s => s.RequestType.NameAr)
            .Map(d => d.RequestTypeNameEn, s => s.RequestType.NameEn)
            .Map(d => d.StatusNameAr, s => s.Status.NameAr)
            .Map(d => d.StatusNameEn, s => s.Status.NameEn)
            .Map(d => d.QuestionBankTypeId, s => s.QuestionBank.QuestionBankTypeId)
            .Map(d => d.QuestionBankTypeNameAr, s => s.QuestionBank.QuestionBankType.NameAr)
            .Map(d => d.QuestionBankTypeNameEn, s => s.QuestionBank.QuestionBankType.NameEn)
            .Map(d => d.ManagementId, s => s.QuestionBank.ManagementId)
            .Map(d => d.ManagementNameAr, s => s.QuestionBank.Management == null ? null : s.QuestionBank.Management.NameAr)
            .Map(d => d.ManagementNameEn, s => s.QuestionBank.Management == null ? null : s.QuestionBank.Management.NameEn)
            .Map(d => d.JobTitleId, s => s.QuestionBank.JobTitleId)
            .Map(d => d.JobTitleNameAr, s => s.QuestionBank.JobTitle == null ? null : s.QuestionBank.JobTitle.JobNameAr)
            .Map(d => d.JobTitleNameEn, s => s.QuestionBank.JobTitle == null ? null : s.QuestionBank.JobTitle.JobNameEn)
            .Map(d => d.SubmittedByNameAr, s => s.SubmittedBy.FullNameAr)
            .Map(d => d.SubmittedByNameEn, s => s.SubmittedBy.FullNameEn);
    }
}
