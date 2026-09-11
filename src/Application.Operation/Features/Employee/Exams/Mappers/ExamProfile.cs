using Application.Operation.Features.Employee.Exams.DTOs;
using Mapster;
using Tawtheef.Domain.Entities.Exams;

namespace Application.Operation.Features.Employee.Exams.Mappers;

public sealed class ExamProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ExamConfigurationDto, Exam>()
            .Ignore(x => x.Id, x => x.ExamNo, x => x.StatusId, x => x.DecisionById,
                x => x.DecisionAt, x => x.DecisionNotes)
            .Map(x => x.TitleAr, x => x.TitleAr.Trim())
            .Map(x => x.TitleEn, x => x.TitleEn == null ? null : x.TitleEn.Trim())
            .Map(x => x.Notes, x => x.Notes == null ? null : x.Notes.Trim());
        config.NewConfig<Exam, ExamConfigurationDto>().Ignore(x => x.Parts);
        config.NewConfig<ExamPartDto, ExamPart>().Ignore(x => x.Id, x => x.ExamId);
        config.NewConfig<ExamPart, ExamPartDto>().Ignore(x => x.Categories);
        config.NewConfig<ExamCategoryDto, ExamCategory>().Ignore(x => x.Id, x => x.ExamPartId);
        config.NewConfig<ExamCategory, ExamCategoryDto>();
    }
}
