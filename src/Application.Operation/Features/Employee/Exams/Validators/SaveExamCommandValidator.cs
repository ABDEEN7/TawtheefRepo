using Application.Operation.Features.Employee.Exams.Commands;
using FluentValidation;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.Exams.Validators;

public sealed class SaveExamCommandValidator : AbstractValidator<SaveExamCommand>
{
    public SaveExamCommandValidator()
    {
        RuleFor(x => x.DraftId).NotEqual(Guid.Empty).When(x => x.DraftId.HasValue);
        RuleFor(x => x.Exam).NotNull();
        When(x => x.Exam != null, () =>
        {
            RuleFor(x => x.Exam.JobId).NotEmpty();
            RuleFor(x => x.Exam.InterruptionPolicyId).NotEmpty();
            RuleFor(x => x.Exam.TitleAr).NotNull().MaximumLength(200);
            RuleFor(x => x.Exam.TitleEn).MaximumLength(200);
            RuleFor(x => x.Exam.Notes).MaximumLength(2000);
            RuleFor(x => x.Exam.TotalQuestions).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Exam.Parts).NotNull();
            RuleFor(x => x).Must(StructurallyValid).WithMessage(ErrorsCodes.InvalidRequest);
            When(x => x.Submit, () =>
            {
                RuleFor(x => x.Exam.TitleAr).NotEmpty();
                RuleFor(x => x).Must(Complete).WithMessage(ErrorsCodes.InvalidRequest);
            });
        });
    }

    private static bool StructurallyValid(SaveExamCommand request)
    {
        var parts = request.Exam.Parts;
        if (parts == null || parts.Count != 2 || parts.Any(p => p == null || p.Categories == null)) return false;
        if (parts[0].PartNo != 1 || parts[1].PartNo != 2 || parts[0].Categories.Count != 1 ||
            parts[0].Categories[0].CategoryId != ExamCategoryTypeIds.Specialized ||
            parts[1].Categories.Any(c => c.CategoryId == ExamCategoryTypeIds.Specialized) ||
            parts[1].Categories.Select(c => c.CategoryId).Distinct().Count() != parts[1].Categories.Count)
            return false;
        return parts.All(p => p.DurationMinutes >= 0 &&
            p.TitleAr != null && p.TitleAr.Length <= 200 && (p.TitleEn?.Length ?? 0) <= 200 &&
            (p.QualificationScore == null || p.QualificationScore is >= 0 and <= 100) &&
            p.Categories.All(c => c != null && c.CategoryId != Guid.Empty &&
                c.QuestionBankVersionId != Guid.Empty && c.QuestionCount >= 0 &&
                c.WeightPercent is >= 0 and <= 100 && decimal.Round(c.WeightPercent, 2) == c.WeightPercent &&
                c.EasyQuestionCount >= 0 && c.MediumQuestionCount >= 0 && c.HardQuestionCount >= 0) &&
            p.Categories.Select(c => c.CategoryId).Distinct().Count() == p.Categories.Count) &&
            parts.SelectMany(p => p.Categories).Sum(c => (long)c.QuestionCount) <= int.MaxValue;
    }

    private static bool Complete(SaveExamCommand request)
    {
        if (!StructurallyValid(request)) return false;
        var parts = request.Exam.Parts;
        var categories = parts.SelectMany(p => p.Categories).ToList();
        return parts.All(p => p.DurationMinutes > 0 && !string.IsNullOrWhiteSpace(p.TitleAr) &&
                p.Categories.Count > 0) &&
            categories.All(c => c.QuestionCount > 0 &&
                (long)c.EasyQuestionCount + c.MediumQuestionCount + c.HardQuestionCount == c.QuestionCount) &&
            categories.Sum(c => c.WeightPercent) == 100m &&
            categories.Sum(c => (long)c.QuestionCount) == request.Exam.TotalQuestions;
    }
}
